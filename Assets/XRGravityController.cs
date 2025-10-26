using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(CharacterController))]
public class XRGravityController : MonoBehaviour
{
    [Header("Gravedad")]
    public float gravity = -9.81f;
    public float groundedOffset = 0.05f; // margen para detectar suelo
    public LayerMask groundLayer;

    private CharacterController characterController;
    private XROrigin xrOrigin;
    private float verticalVelocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        xrOrigin = GetComponent<XROrigin>();
    }

    void Update()
    {
        // Detección del suelo
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 
                                          characterController.height / 2 + groundedOffset, 
                                          groundLayer);

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // mantiene contacto con el suelo
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // aplica gravedad
        }

        // Mueve el XR Rig según la gravedad
        Vector3 move = new Vector3(0, verticalVelocity, 0);
        characterController.Move(move * Time.deltaTime);
    }
}