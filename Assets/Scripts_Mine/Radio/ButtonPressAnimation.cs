using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonPressAnimation : MonoBehaviour
{
    public float pressDistance = 0.01f; // Qué tanto se mueve en +X al presionar
    public float speed = 12f;           // Velocidad del movimiento

    private Vector3 originalPos;
    private bool isPressed = false;

    void Start()
    {
        originalPos = transform.localPosition;

        // Conectar eventos automáticamente
        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener((a) => Press());
            interactable.selectExited.AddListener((a) => Release());
        }
    }

    void Update()
    {
        Vector3 target;

        if (isPressed)
        {
            // Se mueve hacia +X
            target = originalPos + new Vector3(pressDistance, 0, 0);
        }
        else
        {
            // Vuelve a su posición original
            target = originalPos;
        }

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            target,
            Time.deltaTime * speed
        );
    }

    public void Press()
    {
        isPressed = true;
    }

    public void Release()
    {
        isPressed = false;
    }
}

