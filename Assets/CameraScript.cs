using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject sun;
    private Camera cam;
    private System.Random rnd;


    private void randomizeCamera()
    {

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
