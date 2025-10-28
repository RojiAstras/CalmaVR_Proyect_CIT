using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;

public class PaperCrumple : MonoBehaviour
{
    [Header("Configuracion")]
    public GameObject sphereVersion; //referencia a la version esferica del papel
    public float requiredTouchTime = 0.2f;
    public string leftHandTag = "Left Hand";
    public string rightHandTag = "Right Hand";

    private XRGrabInteractable grabInteractable;
    private string holdingHandTag = "";
    private float touchTimer = 0f;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }
 
    void OnGrab(SelectEnterEventArgs args)
    {

        //Derecta que mano lo tomo
        var interactor = args.interactorObject.transform;
        if (interactor.CompareTag(leftHandTag))
        {
            holdingHandTag = leftHandTag;
            Debug.Log("Left hold");
        }
        else if (interactor.CompareTag(rightHandTag))
        {
            holdingHandTag = rightHandTag;
            Debug.Log("Right hold");
        }
        else
            holdingHandTag = ""; //por si acaso
        
    }

    void OnRelease(SelectExitEventArgs args)
    {
        holdingHandTag = "";
    }

    private void OnTriggerStay(Collider other)
    {
        //if (isHeld || isCrumpled) return;

        //Si la otramano toca el papel (la opuesta a la que lo sostiene
        bool isOtherHand =
            (holdingHandTag == leftHandTag && other.CompareTag(rightHandTag)) ||
            (holdingHandTag == rightHandTag && other.CompareTag(leftHandTag));

        if (isOtherHand)
        {
            touchTimer += Time.deltaTime;
            if (touchTimer >= requiredTouchTime)
                CrumplePaper();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(leftHandTag) || other.CompareTag(rightHandTag))
            touchTimer = 0f;
    }

    void CrumplePaper()
    {

        //Crear esfera en la misma posicion y rotacion
        GameObject newSphere = Instantiate(sphereVersion, transform.position, transform.rotation);
        //newSphere.transform.localScale = transform.localScale;

        //Transferir la interaccion al nuevo objeto
        Destroy(gameObject);
    }
}
