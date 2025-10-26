using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This script ensures the VR locomotion system is properly configured for continuous movement
/// Attach this to the XR Origin to enable walking in VR
/// </summary>
public class VRLocomotionSetup : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed of continuous movement")]
    public float moveSpeed = 2.0f;
    
    [Tooltip("Speed of turning")]
    public float turnSpeed = 60.0f;
    
    void Start()
    {
        SetupLocomotion();
    }
    
    void SetupLocomotion()
    {
        // Find or add Continuous Move Provider
        ContinuousMoveProviderBase moveProvider = GetComponentInChildren<ContinuousMoveProviderBase>();
        if (moveProvider != null)
        {
            moveProvider.moveSpeed = moveSpeed;
            Debug.Log("Continuous Move Provider configured with speed: " + moveSpeed);
        }
        else
        {
            Debug.LogWarning("No Continuous Move Provider found. Make sure XR Origin has locomotion configured.");
        }
        
        // Find or add Continuous Turn Provider
        ContinuousTurnProviderBase turnProvider = GetComponentInChildren<ContinuousTurnProviderBase>();
        if (turnProvider != null)
        {
            turnProvider.turnSpeed = turnSpeed;
            Debug.Log("Continuous Turn Provider configured with speed: " + turnSpeed);
        }
        
        // Ensure Character Controller exists for collision
        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.3f;
            characterController.center = new Vector3(0, 0.9f, 0);
            Debug.Log("Character Controller added to XR Origin for collision detection");
        }
    }
}
