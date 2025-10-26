using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTriggerUI : MonoBehaviour
{
    [Header("Referencia el Canvas que se activara")]
    public GameObject canvasUI;

    [Header("Etiqueta del objeto que activa el area (XR Origin o MainCamera)")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag)) 
        {
            if (canvasUI != null)
            {
                canvasUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (canvasUI != null)
            {
                canvasUI.SetActive(false);
            }
        }
    }
}
