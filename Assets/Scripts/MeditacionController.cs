using UnityEngine;
using UnityEngine.UI;

public class MeditacionController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject meditacionUI; // Asignar el Canvas de la UI
    public AudioClip audioMeditacion; // Asignar el audio de meditación
    
    [Header("Configuración")]
    public float distanciaUI = 3f; // Distancia a la que se activa la UI
    public KeyCode teclaInteraccion = KeyCode.E; // Tecla para interactuar
    
    private AudioSource audioSource;
    private bool isPlayerInRange = false;
    private Transform player;
    private bool uiEstaActiva = false;

    private void Start()
    {
        // Buscar al jugador automáticamente (asegúrate de que tenga el tag "Player")
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Configurar AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true; // Para que la meditación se repita
        
        // Asegurarse de que la UI esté desactivada al inicio
        if (meditacionUI != null)
        {
            meditacionUI.SetActive(false);
        }
        else
        {
            Debug.LogError("No se ha asignado el objeto de la UI en el inspector.");
        }
        
        // Verificar que se haya asignado el audio
        if (audioMeditacion == null)
        {
            Debug.LogError("No se ha asignado el audio de meditación en el inspector.");
        }
    }

    private void Update()
    {
        if (player == null) return;
        
        // Calcular distancia al jugador
        float distanciaAlJugador = Vector3.Distance(transform.position, player.position);
        
        // Actualizar estado de rango
        bool estabaEnRango = isPlayerInRange;
        isPlayerInRange = distanciaAlJugador <= distanciaUI;
        
        // Mostrar/ocultar UI cuando el jugador entra/sale del rango
        if (isPlayerInRange != estabaEnRango)
        {
            if (isPlayerInRange)
            MostrarUI();
            else
                OcultarUI();
        }
        
        // Si el jugador está en rango y presiona la tecla de interacción
        if (isPlayerInRange && Input.GetKeyDown(teclaInteraccion))
        {
            ToggleMeditacion();
        }
    }
    
    private void MostrarUI()
    {
        if (meditacionUI != null && !uiEstaActiva)
        {
            meditacionUI.SetActive(true);
            uiEstaActiva = true;
            Debug.Log("UI de meditación mostrada. Presiona " + teclaInteraccion + " para interactuar.");
        }
    }
    
    private void OcultarUI()
    {
        if (meditacionUI != null && uiEstaActiva)
        {
            // Detener la meditación si está sonando
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                Debug.Log("Meditación detenida (jugador se alejó).");
            }
            
            meditacionUI.SetActive(false);
            uiEstaActiva = false;
        }
    }
    
    public void ToggleMeditacion()
    {
        if (audioSource.isPlaying)
        {
            DetenerMeditacion();
        }
        else
        {
            IniciarMeditacion();
        }
    }
    
    public void IniciarMeditacion()
    {
        if (audioMeditacion != null)
        {
            audioSource.clip = audioMeditacion;
            audioSource.Play();
            Debug.Log("Meditación iniciada: " + audioMeditacion.name);
            
            // Actualizar UI si es necesario
            // Por ejemplo, cambiar el texto del botón a "Detener Meditación"
        }
    }
    
    public void DetenerMeditacion()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Meditación detenida.");
            
            // Actualizar UI si es necesario
            // Por ejemplo, cambiar el texto del botón a "Comenzar Meditación"
        }
    }
    
    // Método para ser llamado desde el botón de la UI
    public void BotonComenzarDetener()
    {
        ToggleMeditacion();
    }
    
    // Dibujar un gizmo en el editor para visualizar el área de interacción
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanciaUI);
    }
}
