using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class WhiteboardMarkerXR : MonoBehaviour
{
    [Header("Marker Settings")]
    [SerializeField] private Transform _tip;
    [SerializeField] private int _penSize = 5;
    [SerializeField] private int _minPenSize = 1;
    [SerializeField] private int _maxPenSize = 30;

    [Header("UI Feedback")]
    [SerializeField] private Canvas sizeCanvas; // Canvas en World Space
    [SerializeField] private TMP_Text sizeText; // Texto que muestra el tamaño
    [SerializeField] private float uiHideDelay = 1.5f;

    [Header("Input Actions")]
    [SerializeField] private InputActionProperty IncreaseBrushSizeAction;
    [SerializeField] private InputActionProperty DecreaseBrushSizeAction;

    private Renderer _renderer;
    private Color[] _colors;
    private float _tipHeight;
    private RaycastHit _touch;
    private Whiteboard _whiteboard;
    private Vector2 _touchPos, _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;
    private XRGrabInteractable grabInteractable;
    private Coroutine hideUICoroutine;

    void Start()
    {
        _renderer = _tip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
        _tipHeight = _tip.localScale.y;

        grabInteractable = GetComponent<XRGrabInteractable>();

        if (sizeCanvas != null)
            sizeCanvas.gameObject.SetActive(false);

        // Suscribirse a eventos de Input
        if (IncreaseBrushSizeAction.action != null)
            IncreaseBrushSizeAction.action.performed += OnIncreaseBrush;

        if (DecreaseBrushSizeAction.action != null)
            DecreaseBrushSizeAction.action.performed += OnDecreaseBrush;
    }

    private void OnDestroy()
    {
        // Desuscribirse correctamente
        if (IncreaseBrushSizeAction.action != null)
            IncreaseBrushSizeAction.action.performed -= OnIncreaseBrush;

        if (DecreaseBrushSizeAction.action != null)
            DecreaseBrushSizeAction.action.performed -= OnDecreaseBrush;
    }

    void Update()
    {
        Draw();
    }

    private void Draw()
    {
        if (Physics.Raycast(_tip.position, transform.up, out _touch, _tipHeight))
        {
            if (_touch.transform.CompareTag("Whiteboard"))
            {
                if (_whiteboard == null)
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>();

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                int x = (int)(_touchPos.x * _whiteboard.textureSize.x - (_penSize / 2));
                int y = (int)(_touchPos.y * _whiteboard.textureSize.y - (_penSize / 2));

                if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x) return;

                if (_touchedLastFrame)
                {
                    // Actualizar el color actual del marcador
                    _colors = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
                    int halfPen = _penSize / 2;
                    _whiteboard.texture.SetPixels(x - halfPen, y - halfPen, _penSize, _penSize, _colors);

                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        int lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        int lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _whiteboard.texture.SetPixels(lerpX, lerpY, _penSize, _penSize, _colors);
                    }

                    transform.rotation = _lastTouchRot;
                    _whiteboard.texture.Apply();
                }

                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
        }

        _whiteboard = null;
        _touchedLastFrame = false;
    }

    private void OnIncreaseBrush(InputAction.CallbackContext context)
    {
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            _penSize = Mathf.Min(_penSize + 1, _maxPenSize);
            ApplyBrushSizeChange();
        }
    }

    private void OnDecreaseBrush(InputAction.CallbackContext context)
    {
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            _penSize = Mathf.Max(_penSize - 1, _minPenSize);
            ApplyBrushSizeChange();
        }
    }

    private void ApplyBrushSizeChange()
    {
        _colors = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
        ShowSizeUI();
    }

    private void ShowSizeUI()
    {
        if (sizeCanvas == null || sizeText == null) return;

        sizeCanvas.gameObject.SetActive(true);
        sizeText.text = $"{_penSize}";

        if (hideUICoroutine != null)
            StopCoroutine(hideUICoroutine);

        hideUICoroutine = StartCoroutine(HideUIDelay());
    }

    private IEnumerator HideUIDelay()
    {
        yield return new WaitForSeconds(uiHideDelay);
        if (sizeCanvas != null)
            sizeCanvas.gameObject.SetActive(false);
    }
}
