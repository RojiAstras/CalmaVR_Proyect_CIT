using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class ObjetoLunar : MonoBehaviour
{
    public float tiempoVolumen = 10f;
    private bool flagVolumen = true; 
    private AudioSource sonidoColision;
    private ConstantForce cForce;
    private UnityEngine.Vector3 forceDirection;

    // Start is called before the first frame update
    void Start()
    {
        sonidoColision = GetComponent<AudioSource>();
        cForce = GetComponent<ConstantForce>();
        forceDirection = new UnityEngine.Vector3(0, -1.5f, 0);
        cForce.force = forceDirection;
    }

    // Update is called once per frame
    void Update()
    {
        if (tiempoVolumen > 0)
        {
            tiempoVolumen -= Time.deltaTime;
        }

        if (tiempoVolumen <= 0 && flagVolumen)
        {
            GetComponent<AudioSource>().mute = false;
            flagVolumen = false;
        }
        

    }

    void OnCollisionEnter(Collision collision)
    {
        sonidoColision.Play();
    }
}
