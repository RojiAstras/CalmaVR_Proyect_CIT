using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_script : MonoBehaviour
{
    //Variables públicas
    public float speedMovement = 2f;

    //Variables Privadas
    private UnityEngine.Vector3 initialPosition;
    float xAxis = 0, zAxis = 0;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speedMovement = speedMovement * 4;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speedMovement = speedMovement / 4;
        }


        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)) // W y S presionadas
        {
            zAxis = 0;
        }
        else if (Input.GetKey(KeyCode.W)) // Solo W presionada
        {
            zAxis = 1;
        }
        else if (Input.GetKey(KeyCode.S)) // Solo S presionada
        {
            zAxis = -1;
        }
        else // Sin Input
        {
            zAxis = 0;
        }

        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) // A y D presionadas
        {
            xAxis = 0;
        }
        else if (Input.GetKey(KeyCode.D)) // Solo D presionada
        {
            xAxis = 1;
        }
        else if (Input.GetKey(KeyCode.A)) // Solo A presionada
        {
            xAxis = -1;
        }
        else // Sin Input
        {
            xAxis = 0;
        }
        
        transform.Translate(new UnityEngine.Vector3(xAxis, 0, zAxis) * Time.deltaTime * speedMovement);
    }
}
