using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjetoLunar : MonoBehaviour
{

    public float fuerzaGravedad = 5;
    public float tiempoVolumen = 10f;


    private UnityEngine.Vector3 PosInicial;
    private bool flagVolumen = true; 
    private AudioSource sonidoColision;
    private ConstantForce cForce;
    private UnityEngine.Vector3 forceDirection;
    private Rigidbody rbody;
    private GameObject eQuad;

    // Start is called before the first frame update
    void Start()
    {
        PosInicial = gameObject.transform.position;
        sonidoColision = GetComponent<AudioSource>();
        cForce = GetComponent<ConstantForce>();
        forceDirection = new UnityEngine.Vector3(0, -fuerzaGravedad, 0);
        cForce.force = forceDirection;
        rbody = GetComponent<Rigidbody>();
        eQuad = GameObject.Find("EventQuad");

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

        if (Input.GetKeyDown(KeyCode.Space) && name == "Item_Neptuno")
        {
            transform.position = PosInicial;
        }

    }

    void OnCollisionEnter(Collision collision) //Detectar Colisiones
    {
        if (collision.gameObject == eQuad) // Colisiona con el techo 
        {
            cForce.force = UnityEngine.Vector3.zero;
            rbody.velocity = UnityEngine.Vector3.zero;
            transform.Translate(new(0, 50, 0));
            transform.localScale = new(15, 15, 15);
            transform.eulerAngles = new(65, 0, 0);
            
        }
        else
        {
            sonidoColision.Play();
        }
        
    }

}
