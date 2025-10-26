using UnityEngine;

/// <summary>
/// Previene que el jugador se caiga del terreno
/// Añade límites invisibles alrededor del área caminable
/// </summary>
public class TerrainBoundaryProtection : MonoBehaviour
{
    [Header("Boundary Settings")]
    [Tooltip("Tamaño del área segura (debe coincidir con el tamaño del terreno)")]
    public Vector2 safeAreaSize = new Vector2(100f, 100f);
    
    [Tooltip("Centro del área segura")]
    public Vector2 safeAreaCenter = Vector2.zero;
    
    [Tooltip("Altura mínima permitida (previene caídas)")]
    public float minimumHeight = -5f;
    
    [Tooltip("Posición de respawn si el jugador se cae")]
    public Vector3 respawnPosition = new Vector3(0, 2, 0);
    
    private Transform playerTransform;
    private CharacterController characterController;
    
    void Start()
    {
        // Find the player (XR Origin)
        playerTransform = transform;
        characterController = GetComponent<CharacterController>();
        
        // If this is attached to XR Origin, we're good
        if (playerTransform.name.Contains("XR Origin") || playerTransform.name.Contains("XR Rig"))
        {
            Debug.Log("Terrain Boundary Protection active");
        }
        else
        {
            Debug.LogWarning("TerrainBoundaryProtection should be attached to XR Origin");
        }
    }
    
    void Update()
    {
        CheckBoundaries();
    }
    
    void CheckBoundaries()
    {
        Vector3 currentPos = playerTransform.position;
        bool needsCorrection = false;
        Vector3 correctedPos = currentPos;
        
        // Check horizontal boundaries
        float halfWidth = safeAreaSize.x / 2f;
        float halfDepth = safeAreaSize.y / 2f;
        
        // X boundary
        if (currentPos.x < safeAreaCenter.x - halfWidth)
        {
            correctedPos.x = safeAreaCenter.x - halfWidth + 0.5f;
            needsCorrection = true;
        }
        else if (currentPos.x > safeAreaCenter.x + halfWidth)
        {
            correctedPos.x = safeAreaCenter.x + halfWidth - 0.5f;
            needsCorrection = true;
        }
        
        // Z boundary
        if (currentPos.z < safeAreaCenter.y - halfDepth)
        {
            correctedPos.z = safeAreaCenter.y - halfDepth + 0.5f;
            needsCorrection = true;
        }
        else if (currentPos.z > safeAreaCenter.y + halfDepth)
        {
            correctedPos.z = safeAreaCenter.y + halfDepth - 0.5f;
            needsCorrection = true;
        }
        
        // Check if player fell below minimum height
        if (currentPos.y < minimumHeight)
        {
            Debug.LogWarning("Player fell below minimum height! Respawning...");
            correctedPos = respawnPosition;
            needsCorrection = true;
        }
        
        // Apply correction
        if (needsCorrection)
        {
            if (characterController != null)
            {
                characterController.enabled = false;
                playerTransform.position = correctedPos;
                characterController.enabled = true;
            }
            else
            {
                playerTransform.position = correctedPos;
            }
        }
    }
    
    // Draw boundaries in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        
        // Draw safe area boundaries
        Vector3 center = new Vector3(safeAreaCenter.x, 0, safeAreaCenter.y);
        Vector3 size = new Vector3(safeAreaSize.x, 0.1f, safeAreaSize.y);
        
        Gizmos.DrawWireCube(center, size);
        
        // Draw minimum height plane
        Gizmos.color = Color.red;
        Vector3 minHeightCenter = new Vector3(safeAreaCenter.x, minimumHeight, safeAreaCenter.y);
        Gizmos.DrawWireCube(minHeightCenter, size);
        
        // Draw respawn point
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(respawnPosition, 0.5f);
    }
}
