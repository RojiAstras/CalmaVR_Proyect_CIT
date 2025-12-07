using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class VRSpinnerDualPro : MonoBehaviour
{
    [Header("Configuración del Giro")]
    public Vector3 localSpinAxis = Vector3.up;
    public float maxSpinTorque = 200f;  // Torque máximo aplicado
    [Range(0f, 1f)] public float friction = 0.995f;
    public float maxAngularVelocity = 100f;

    [Header("Entradas (VR)")]
    public InputActionProperty leftTriggerAction;
    public InputActionProperty rightTriggerAction;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool isLeftGrabbing = false;
    private bool isRightGrabbing = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        rb.maxAngularVelocity = maxAngularVelocity;
        rb.drag = 0f;
        rb.angularDrag = 0.02f;

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void FixedUpdate()
    {
        // --- Fijar fricción angular ---
        if (rb.angularVelocity.magnitude > 0.001f)
            rb.angularVelocity *= friction;

        // --- Lectura de triggers ---
        float leftTrigger = leftTriggerAction.action?.ReadValue<float>() ?? 0f;
        float rightTrigger = rightTriggerAction.action?.ReadValue<float>() ?? 0f;

        // Device Simulator
        if (Keyboard.current.hKey.isPressed) leftTrigger = 1f;
        if (Keyboard.current.jKey.isPressed) rightTrigger = 1f;

        // --- Aplicar torque proporcional ---
        if (isLeftGrabbing)
        {
            ApplySpinTorque(rightTrigger); // acelera según presión trigger derecho
        }
        else if (isRightGrabbing)
        {
            ApplySpinTorque(-leftTrigger); // acelera según presión trigger izquierdo
        }
        else
        {
            // En editor, puede girar sin agarrar
            ApplySpinTorque(rightTrigger);  // tecla J
            ApplySpinTorque(-leftTrigger);  // tecla H
        }
    }

    private void ApplySpinTorque(float triggerValue)
    {
        if (Mathf.Abs(triggerValue) < 0.01f) return; // No aplicar si casi 0

        Vector3 worldAxis = transform.TransformDirection(localSpinAxis.normalized);
        rb.AddTorque(worldAxis * triggerValue * maxSpinTorque, ForceMode.VelocityChange);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        string name = args.interactorObject.transform.name.ToLower();
        isLeftGrabbing = name.Contains("left");
        isRightGrabbing = name.Contains("right");
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isLeftGrabbing = false;
        isRightGrabbing = false;
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }
}
