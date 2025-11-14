using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class ColorPickerXR : MonoBehaviour
{
    [Header("Configuración XR")]
    public XRRayInteractor rayInteractor;      // Rayo del controlador XR
    public InputActionProperty selectAction;   // Acción del gatillo (Trigger)

    [Header("Objetivos")]
    public Renderer targetRenderer;            // Objeto a colorear (por ejemplo, un cubo)
    public Texture2D colorTexture;             // Textura de la rueda (opcional)

    private bool isSelecting = false;
    private Renderer thisRenderer;

    void Start()
    {
        thisRenderer = GetComponent<Renderer>();

        // Si no se asigna la textura manualmente, intenta obtenerla del material del objeto
        if (colorTexture == null && thisRenderer != null && thisRenderer.material.mainTexture != null)
        {
            // Intentamos convertir la textura del material a Texture2D
            colorTexture = thisRenderer.material.mainTexture as Texture2D;

            if (colorTexture == null)
            {
                Debug.LogWarning("⚠ La textura del material no es una Texture2D o no tiene 'Read/Write Enabled'.");
            }
        }

        if (selectAction != null)
        {
            selectAction.action.performed += OnSelectPerformed;
            selectAction.action.canceled += OnSelectCanceled;
        }
    }

    void OnDestroy()
    {
        if (selectAction != null)
        {
            selectAction.action.performed -= OnSelectPerformed;
            selectAction.action.canceled -= OnSelectCanceled;
        }
    }

    void OnSelectPerformed(InputAction.CallbackContext ctx) => isSelecting = true;
    void OnSelectCanceled(InputAction.CallbackContext ctx) => isSelecting = false;

    void Update()
    {
        if (rayInteractor == null || colorTexture == null)
            return;

        // Verificamos si el rayo golpea algo
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // Solo si golpea este mismo objeto
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Vector2 uv = hit.textureCoord;
                Color color = colorTexture.GetPixelBilinear(uv.x, uv.y);

                if (isSelecting && targetRenderer != null)
                {
                    targetRenderer.material.color = color;
                }
            }
        }
    }
}
