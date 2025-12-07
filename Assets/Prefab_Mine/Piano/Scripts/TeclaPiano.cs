using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(AudioSource))]
public class VRPianoKey : XRBaseInteractable
{
    [Header("Configuración de sonido")]
    public AudioClip pianoSound;

    [Header("Movimiento de tecla")]
    [Tooltip("Distancia hacia abajo que baja la tecla al presionarse.")]
    public float pressDepth = 0.02f;
    [Tooltip("Velocidad a la que vuelve a su posición original.")]
    public float returnSpeed = 15f;

    [Header("Configuración de posición")]
    [Tooltip("Posición original de la tecla (se toma del editor si overrideOriginalPosition = false).")]
    [SerializeField] private Vector3 originalPosition;
    [Tooltip("Si está activado, se usará la posición definida manualmente en el campo anterior.")]
    [SerializeField] private bool overrideOriginalPosition = false;

    private AudioSource audioSource;
    private bool isPressed = false;
    private float hoverExitDelay = 0.8f;  // Tiempo que espera antes de soltar la tecla
    private float hoverTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();

        // Solo usar la posición del editor si no se sobrescribe manualmente
        if (!overrideOriginalPosition)
            originalPosition = transform.localPosition;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (isHovered)
        {
            // Reinicia el temporizador y presiona si aún no está presionada
            hoverTimer = 0f;
            if (!isPressed) PressKey();
        }
        else
        {
            // Aumenta el temporizador cuando ya no hay hover
            hoverTimer += Time.deltaTime;

            // Solo libera la tecla si ha pasado el tiempo de tolerancia
            if (isPressed && hoverTimer >= hoverExitDelay)
                ReleaseKey();
        }

        // Movimiento de retorno suave (igual que antes)
        if (!isPressed)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * returnSpeed);
        }
    }

    private void PressKey()
    {
        isPressed = true;

        // Siempre calcular hundimiento RELATIVO a la posición original
        Vector3 targetPosition = originalPosition - new Vector3(0, pressDepth, 0);
        transform.localPosition = targetPosition;

        // Reproducir sonido
        if (pianoSound != null)
            audioSource.PlayOneShot(pianoSound);
    }

    private void ReleaseKey()
    {
        isPressed = false;
    }
}
