using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class controlCamara : MonoBehaviour
{
    float rotationSpeed = 70;
    Vector3 currentEulerAngles;
    float x;
    float y;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        //modifying the Vector3, based on input multiplied by speed and time
        currentEulerAngles += new Vector3(-y, x, 0) * Time.deltaTime * rotationSpeed;

        //apply the change to the gameObject
        transform.eulerAngles = currentEulerAngles;

        if (Input.GetKey(KeyCode.K))
        {
            transform.Translate(new Vector3(0, 0, -2) * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.I))
        {
            transform.Translate(new Vector3(0, 0, 2) * Time.deltaTime);
        }
        
    }
}