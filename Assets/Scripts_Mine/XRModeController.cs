using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Controla cuándo cada mano está en modo "interacción" (Grab/UI) o "teletransporte",
/// desactivando completamente los componentes y visuales correspondientes.
/// </summary>
public class XRModeController : MonoBehaviour
{
    [Header("Left Hand")]
    public XRRayInteractor leftGrabRay;          // El rayo de interacción (UI / objetos)
    public GameObject leftGrabVisual;            // Su línea visual (opcional)
    public XRRayInteractor leftTeleportRay;      // El rayo de teletransporte
    public GameObject leftTeleportVisual;        // Su línea visual (curva)

    [Header("Right Hand")]
    public XRRayInteractor rightGrabRay;
    public GameObject rightGrabVisual;
    public XRRayInteractor rightTeleportRay;
    public GameObject rightTeleportVisual;

    [Header("Input Actions")]
    public InputActionProperty leftTeleportActivate;   // Botón de teleport
    public InputActionProperty rightTeleportActivate;
    public InputActionProperty leftTeleportCancel;     // Botón de cancelar teleport (grip)
    public InputActionProperty rightTeleportCancel;

    [Header("Configuración")]
    [Tooltip("Tiempo mínimo (s) de pulsación para considerar 'mantener' teleport")]
    public float teleportHoldThreshold = 0.08f;

    private float leftTeleportHoldTime;
    private float rightTeleportHoldTime;

    void Update()
    {
        // ----- LEFT -----
        bool leftCancelPressed = leftTeleportCancel.action.ReadValue<float>() > 0.1f;
        float leftActivateValue = leftTeleportActivate.action.ReadValue<float>();

        if (leftActivateValue > 0.1f && !leftCancelPressed)
            leftTeleportHoldTime += Time.deltaTime;
        else
            leftTeleportHoldTime = 0f;

        bool leftWantsTeleport = leftTeleportHoldTime >= teleportHoldThreshold;

        // ----- RIGHT -----
        bool rightCancelPressed = rightTeleportCancel.action.ReadValue<float>() > 0.1f;
        float rightActivateValue = rightTeleportActivate.action.ReadValue<float>();

        if (rightActivateValue > 0.1f && !rightCancelPressed)
            rightTeleportHoldTime += Time.deltaTime;
        else
            rightTeleportHoldTime = 0f;

        bool rightWantsTeleport = rightTeleportHoldTime >= teleportHoldThreshold;

        // ----- Cambiar modos -----
        SetLeftMode(teleport: leftWantsTeleport);
        SetRightMode(teleport: rightWantsTeleport);
    }

    private void SetLeftMode(bool teleport)
    {
        // Si quiere teletransportar, apaga el grab y prende el teleport
        leftGrabRay.enabled = !teleport;
        if (leftGrabVisual) leftGrabVisual.SetActive(!teleport);

        leftTeleportRay.enabled = teleport;
        if (leftTeleportVisual) leftTeleportVisual.SetActive(teleport);
    }

    private void SetRightMode(bool teleport)
    {
        rightGrabRay.enabled = !teleport;
        if (rightGrabVisual) rightGrabVisual.SetActive(!teleport);

        rightTeleportRay.enabled = teleport;
        if (rightTeleportVisual) rightTeleportVisual.SetActive(teleport);
    }
}
