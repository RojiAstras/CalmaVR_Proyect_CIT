using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicaEscenario : MonoBehaviour
{   
    public float tiempoEvento = 15f;
    public AudioClip[] listaMusica;

    private bool flagMusica = true;

    private AudioSource aSource;
    private int randomIndex;

    // Start is called before the first frame update
    void Start()
    {
        randomIndex = Random.Range(0, 2);
        aSource = GetComponent<AudioSource>();
        aSource.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (tiempoEvento > 0)
        {
            tiempoEvento -= Time.deltaTime;
        }

        if (tiempoEvento <= 0 && flagMusica)
        {
            tocarMusica(randomIndex);
            flagMusica = false;
        }

        if (!aSource.isPlaying)
        {
            flagMusica = true;
        }
    }

    public void tocarMusica(int Index)
    {
        aSource.PlayOneShot(listaMusica[Index]);
    }
}
