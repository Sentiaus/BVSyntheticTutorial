using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject sun;
    private Camera cam;
    private System.Random rnd;

    readonly private Vector2 AspectRatio = new Vector2(1920, 1080);

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

    }
    const string workingDirectory = "";
    // Start is called before the first frame update
    void Start()
    {
        rnd = new System.Random();
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Normalize method
    public float normalize(float value, float total)
    {
        return value / total;
    }

}
