using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class controlCamara : MonoBehaviour
{
    public float rotationSpeed = 70f;
    private Vector3 currentEulerAngles;
    private float xAxis = 0, yAxis = 0;

    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = new Vector3(329,273,0);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            rotationSpeed = rotationSpeed * 2;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            rotationSpeed = rotationSpeed / 2;
        }

        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow)) // Tecla Arriba y Tecla Abajo presionadas
        {
            xAxis = 0;
        }
        else if (Input.GetKey(KeyCode.UpArrow)) // Solo Tecla Arriba presionada
        {
            xAxis = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow)) // Solo Tecla Abajo presionada
        {
            xAxis = -1;
        }
        else // Sin Input
        {
            xAxis = 0;
        }

        if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow)) // Tecla Izq y Tecla Der presionadas
        {
            yAxis = 0;
        }
        else if (Input.GetKey(KeyCode.RightArrow)) // Solo Tecla Der presionada
        {
            yAxis = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow)) // Solo Tecla Izq presionada
        {
            yAxis = -1;
        }
        else // Sin Input
        {
            yAxis = 0;
        }

        //modifying the Vector3, based on input multiplied by speed and time
        currentEulerAngles += rotationSpeed * Time.deltaTime * new UnityEngine.Vector3(-xAxis, yAxis, 0);

        //apply the change to the gameObject
        transform.eulerAngles = currentEulerAngles;
    }
}