using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

public class ColorPicker : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias")]
    public RawImage colorWheelImage;      // Imagen con la textura de la rueda de color
    public Renderer targetRenderer;       // El cubo u objeto cuyo material cambiará de color

    private Texture2D colorTexture;

    void Start()
    {
        if (colorWheelImage != null)
        {
            // Convertimos la textura en Texture2D para poder leer sus píxeles
            colorTexture = colorWheelImage.texture as Texture2D;
        }
    }

    // Se llama cuando el usuario hace clic en la imagen
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localCursor;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            colorWheelImage.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localCursor);

        Rect rect = colorWheelImage.rectTransform.rect;
        float x = Mathf.InverseLerp(rect.xMin, rect.xMax, localCursor.x);
        float y = Mathf.InverseLerp(rect.yMin, rect.yMax, localCursor.y);

        // Convertimos las coordenadas a posición dentro de la textura
        if (colorTexture != null)
        {
            int texX = Mathf.Clamp((int)(x * colorTexture.width), 0, colorTexture.width - 1);
            int texY = Mathf.Clamp((int)(y * colorTexture.height), 0, colorTexture.height - 1);

            Color selectedColor = colorTexture.GetPixel(texX, texY);
            ApplyColor(selectedColor);
        }
    }

    private void ApplyColor(Color newColor)
    {
        if (targetRenderer != null)
        {
            targetRenderer.material.color = newColor;
        }
    }
}
