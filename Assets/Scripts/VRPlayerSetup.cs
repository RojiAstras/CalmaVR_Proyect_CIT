using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

/// <summary>
/// Configures the VR player's camera and height settings
/// Attach this to the XR Origin (XR Rig) GameObject
/// </summary>
public class VRPlayerSetup : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Player's eye height in meters (default is average human height)")]
    public float playerHeight = 1.6f; // Default height in meters
    
    [Tooltip("Camera Y offset from the floor")]
    public float cameraYOffset = 0.1f;
    
    private XROrigin xrOrigin;
    private Camera xrCamera;
    private CharacterController characterController;
    
    void Start()
    {
        // Get or add required components
        xrOrigin = GetComponent<XROrigin>();
        if (xrOrigin == null)
        {
            Debug.LogError("XROrigin component not found! Make sure this script is attached to the XR Origin (XR Rig) GameObject.");
            return;
        }
        
        // Get the main camera (should be a child of the XR Origin)
        xrCamera = xrOrigin.Camera;
        if (xrCamera == null)
        {
            xrCamera = GetComponentInChildren<Camera>();
            if (xrCamera == null)
            {
                Debug.LogError("No camera found in the XR Origin hierarchy!");
                return;
            }
        }
        
        // Get or add character controller
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
        }
        
        // Set up the XR Origin
        SetupXROrigin();
        
        // Set up the camera
        SetupCamera();
        
        // Set up character controller
        SetupCharacterController();
    }
    
    void SetupXROrigin()
    {
        // Make sure tracking space is set to floor
        xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;
        
        // Set camera floor offset
        xrOrigin.CameraYOffset = playerHeight - cameraYOffset;
        
        // Make sure the camera is enabled for XR
        xrOrigin.Camera = xrCamera;
        
        Debug.Log($"XR Origin configured with floor offset: {xrOrigin.CameraYOffset}m");
    }
    
    void SetupCamera()
    {
        if (xrCamera == null) return;
        
        // Ensure the camera is set up for VR
        xrCamera.nearClipPlane = 0.1f;
        xrCamera.farClipPlane = 1000f;
        xrCamera.stereoTargetEye = StereoTargetEyeMask.Both;
        
        // Position the camera at the player's eye level
        xrCamera.transform.localPosition = new Vector3(0, playerHeight, 0);
        
        Debug.Log($"Camera configured at height: {playerHeight}m");
    }
    
    void SetupCharacterController()
    {
        if (characterController == null) return;
        
        // Configure the character controller to match the player's size
        characterController.height = playerHeight;
        characterController.radius = 0.2f;
        characterController.center = new Vector3(0, playerHeight / 2f, 0);
        characterController.skinWidth = 0.08f;
        characterController.minMoveDistance = 0.001f;
        
        // Enable the character controller
        characterController.enabled = true;
        
        Debug.Log($"Character Controller configured with height: {characterController.height}m");
    }
    
    // Call this method to adjust the player's height during runtime if needed
    public void AdjustPlayerHeight(float newHeight)
    {
        if (newHeight <= 0) return;
        
        playerHeight = newHeight;
        
        // Update the camera position
        if (xrCamera != null)
        {
            xrCamera.transform.localPosition = new Vector3(0, playerHeight, 0);
        }
        
        // Update the character controller
        if (characterController != null)
        {
            characterController.height = playerHeight;
            characterController.center = new Vector3(0, playerHeight / 2f, 0);
        }
        
        // Update the XR Origin floor offset
        if (xrOrigin != null)
        {
            xrOrigin.CameraYOffset = playerHeight - cameraYOffset;
        }
        
        Debug.Log($"Player height adjusted to: {playerHeight}m");
    }
}
