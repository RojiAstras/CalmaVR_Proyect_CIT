using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;

public class PaperCrumpleAnimated : MonoBehaviour
{
    [Header("Configuracion General")]
    public GameObject sphereVersion; //referencia a la version esferica del papel
    public float requiredTouchTime = 0.2f;
    public string leftHandTag = "Left Hand";
    public string rightHandTag = "Right Hand";

    [Header("Efectos Visuales")]
    public float shrinkDuration = 0.3f; // Duracion del encogimiento Visual
    public float shrinkScale = 0.4f; // tamaño minimo antes de convertse en esfera

    [Header("Sonido (Opcional)")]
    public AudioClip crumpleSound;
    public AudioSource audioSource;

    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;
    private bool isCrumpled = false;
    private string holdingHandTag = "";
    private float touchTimer = 0f;
    private Coroutine shrinkRoutine;

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
        isHeld = true;

        //Derecta que mano lo tomo
        var interactor = args.interactorObject.transform;
        if (interactor.CompareTag(leftHandTag))
            holdingHandTag = leftHandTag;
        else if (interactor.CompareTag(rightHandTag))
            holdingHandTag = rightHandTag;
        else
            holdingHandTag = ""; //por si acaso

    }

    void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
        holdingHandTag = "";
        touchTimer = 0f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isHeld || isCrumpled) return;

        //Si la otramano toca el papel (la opuesta a la que lo sostiene
        bool isOtherHand =
            (holdingHandTag == leftHandTag && other.CompareTag(rightHandTag)) ||
            (holdingHandTag == rightHandTag && other.CompareTag(leftHandTag));

        if (isOtherHand)
        {
            touchTimer += Time.deltaTime;
            if (touchTimer >= requiredTouchTime)
                StartCrumple();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(leftHandTag) || other.CompareTag(rightHandTag))
            touchTimer = 0f;
    }

    void StartCrumple()
    {
        if (isCrumpled) return;
        isCrumpled = true;

        //reproducir sonido si hay audio configurado
        if (audioSource && crumpleSound)
            audioSource.PlayOneShot(crumpleSound);

        //iniciar animacion de encogimiento
        if (shrinkRoutine != null)
            StopCoroutine(shrinkRoutine);
        shrinkRoutine = StartCoroutine(CrumpleAnimation());
    }

    IEnumerator CrumpleAnimation()
    {
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = initialScale * shrinkScale;

        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkDuration;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            yield return null;

        }

        transform.localScale = targetScale;

        //Crear esfera en la misma posicion y rotacion
        GameObject newSphere = Instantiate(sphereVersion, transform.position, transform.rotation);
        newSphere.transform.localScale = transform.localScale;

        //Transferir la interaccion al nuevo objeto
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}