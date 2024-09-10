using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class CameraScript : MonoBehaviour
{
    public GameObject sun;
    private Camera cam;
    private System.Random rnd;

    public GameObject road;

    public PayloadTargetContents[] payloadTargets;
    public static int picturesTaken = 0;
    readonly public static int totalpics = 4;

    const string workingDirectory = "D:\\Something";
    readonly private Vector2 AspectRatio = new Vector2(1920, 1080);

    private int fileCount = 0;
    private void randomizeCamera()
    {
        float x = rnd.Next(-60, 60);
        float y = rnd.Next(150, 200);
        float z = rnd.Next(-25, 25);
        cam.transform.localPosition = new Vector3(x, y, z);

        float rotation_y = rnd.Next(0, 359);
        float rotation_x = rnd.Next(85, 95);
        cam.transform.localRotation = Quaternion.Euler(rotation_x, rotation_y, 0);
    }

    private void randomizeSun()
    {
        float angle = rnd.Next(0, 180);
        sun.transform.localRotation = Quaternion.Euler(angle, 0, 0);
    }

    // Randomizes location of target (no verification)
    private Vector3 GetRandomPosition()
    {
        float x = rnd.Next(-150, 200); //rnd.Next(-72, 106);
        float z = rnd.Next(-150, 200); //rnd.Next(-73, 60);
        return new Vector3(x, 3.0f, z);
    }

    // Returns a boolean, which verifies if all objects were successfully placed on screen, and not overlapping
    private bool RandomizeAndVerifyTargets()
    {
        bool notFailed = true;
        List<Vector3> placedPositions = new List<Vector3>(); // To store positions of placed objects

        foreach (PayloadTargetContents targetContent in payloadTargets)
        {
            bool isPositionValid = false;
            int attempts = 0;
            int maxAttempts = 10000; // Set a maximum number of attempts to prevent infinite loops

            while (!isPositionValid && attempts < maxAttempts)
            {
                // Randomize position
                Vector3 newPosition = GetRandomPosition();
                targetContent.target.transform.localPosition = newPosition;

                // Check if in view
                Vector3 screenPos = cam.WorldToViewportPoint(targetContent.target.transform.position);
                bool inView = screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1 && screenPos.z > 0;

                // Check for overlaps
                bool isOverlapping = placedPositions.Any(pos => Vector3.Distance(newPosition, pos) < 50f);

                // Update position validity
                isPositionValid = inView && !isOverlapping;

                if (isPositionValid)
                {
                    // Randomize rotation
                    float rotation_y = rnd.Next(0, 359);
                    targetContent.target.transform.localRotation = Quaternion.Euler(0, rotation_y, 2);

                    placedPositions.Add(newPosition); // Add this position to the list
                }

                attempts++;
            }

            if (attempts >= maxAttempts)
            {
                notFailed = false;
                //Debug.LogWarning("Max attempts reached for object placement. Last position may not be ideal.");
            }
        }
        return notFailed;
    }

    private Vector2 GetCenterPosition(GameObject target)
    {
        Vector2 temp = cam.WorldToScreenPoint(target.transform.position);
        Vector2 centerPos = new Vector2(temp.x, AspectRatio.y - temp.y);
        return centerPos;
    }
    // Returns width and height of a payload object as a Vector2 Object
    private Vector2 GetWidthHeight(PayloadTargetContents payload)
    {
        List<Vector2> corners = new List<Vector2>();
        Vector2 temp = cam.WorldToScreenPoint(payload.TLTarget.transform.position);
        corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));
        temp = cam.WorldToScreenPoint(payload.BLTarget.transform.position);
        corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));
        temp = cam.WorldToScreenPoint(payload.TRTarget.transform.position);
        corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));
        temp = cam.WorldToScreenPoint(payload.BRTarget.transform.position);
        corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));

        Vector2 widthHeight = findWidthHeight(corners);
        return widthHeight;
    }   
    //Normalize method
    public float normalize(float value, float total)
    {
        return value / total;
    }

    //Returns Vector2 (width, height) given 4 coordinates
    private Vector2 findWidthHeight(IList<Vector2> corners)
    {
        float highestX = float.MinValue;
        float highestY = float.MinValue;
        float lowestX = float.MaxValue;
        float lowestY = float.MaxValue;

        foreach (Vector2 vector in corners)
        {
            if (vector.x > highestX)
            {
                highestX = vector.x;
            }

            if (vector.y > highestY)
            {
                highestY = vector.y;
            }

            if (vector.x < lowestX)
            {
                lowestX = vector.x;
            }

            if (vector.y < lowestY)
            {
                lowestY = vector.y;
            }
        }

        return new Vector2(highestX - lowestX, highestY - lowestY);
    }

    // //Returns a string comprised of all payload object's class, normalized x and y value, and normalized width and height
    private string GenerateNormalizedDataString()
    {
        string dataString = "";
        Debug.LogWarning("Here");
        foreach (PayloadTargetContents payload in payloadTargets)
        {
            // int materialClass;
            // Renderer targetRenderer = payload.target.GetComponent<Renderer>();
            // if (targetRenderer != null && targetRenderer.material != null)
            // {
            //     string materialName = targetRenderer.material.name;
            //     materialClass = GetMaterialClass(materialName);
            //     dataString += materialClass + " ";
            // }
            Vector2 centerPos = GetCenterPosition(payload.target);
            Vector2 widthHeight = GetWidthHeight(payload);

            dataString += normalize(centerPos.x, AspectRatio.x).ToString() + " " +
                        normalize(centerPos.y, AspectRatio.y).ToString() + " " +
                        normalize(widthHeight.x, AspectRatio.x).ToString() + " " +
                        normalize(widthHeight.y, AspectRatio.y).ToString() + "\n";
            Debug.LogWarning(dataString);
        }

        return dataString;
    }

    
    // Start is called before the first frame update
    void Start()
    {
        rnd = new System.Random();
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(picturesTaken <= totalpics){
            randomizeSun();
            randomizeCamera();
            string textToWrite = GenerateNormalizedDataString();
            string imagePath;
            string labelPath;

            if (picturesTaken % 5 != 0){
                imagePath = workingDirectory + "\\train\\images\\" + (fileCount) + "_" + picturesTaken.ToString()+".png";
                labelPath = workingDirectory + "\\train\\labels\\" + (fileCount) + "_" + picturesTaken.ToString()+".txt";
            }else{
                imagePath = workingDirectory + "\\valid\\images\\" + (fileCount) + "_" + picturesTaken.ToString()+".png";
                labelPath = workingDirectory + "\\valid\\labels\\" + (fileCount) + "_" + picturesTaken.ToString()+".txt";
            }
            // Create a new StreamWriter and write the text to the file
            using (StreamWriter writer = new StreamWriter(labelPath)){
                writer.WriteLine(textToWrite);
            }

            ScreenCapture.CaptureScreenshot(imagePath);
            fileCount++;
            picturesTaken++;
        }
        
    }

    

}
