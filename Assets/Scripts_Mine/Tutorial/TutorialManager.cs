using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("Referencias de controladores")]
    public GameObject leftControllerModel;
    public GameObject rightControllerModel;

    [Header("Pasos del tutorial (en orden)")]
    public GameObject[] tutorialSteps;

    [Header("Acciones de entrada")]
    [Tooltip("Botón lateral de agarre")]
    public InputActionReference gripAction;
    [Tooltip("Gatillo frontal (para interacción y teletransporte)")]
    public InputActionReference triggerAction;
    [Tooltip("Joystick para movimiento")]
    public InputActionReference moveAction;

    [Header("Configuración general")]
    [Tooltip("Si está activado, el tutorial avanza automáticamente después de un tiempo.")]
    public bool avanzarAutomaticamente = false;
    [Tooltip("Segundos que dura cada paso si el modo automático está activado.")]
    public float autoNextDelay = 5f;
    [Tooltip("Tiempo mínimo que se debe mantener presionado el gatillo para completar el teletransporte.")]
    public float teleportHoldTime = 1.0f;

    [Header("Inicio del tutorial")]
    [Tooltip("Retraso (en segundos) antes de comenzar el tutorial al iniciar la escena.")]
    public float startDelay = 2f;

    [Header("Objeto a activar al finalizar el tutorial")]
    public GameObject objectToActivateAtEnd;

    private int currentStep = 0;
    private bool tutorialRunning = false;
    private float timer = 0f;
    private float teleportTimer = 0f;
    private bool triggerHeld = false;

    void Start()
    {
        // Desactivar todos los pasos al inicio
        foreach (var step in tutorialSteps)
            step.SetActive(false);

        // Activar acciones de entrada (aunque aún no inicie el tutorial)
        EnableInputs();

        // Iniciar el tutorial con retraso
        StartCoroutine(StartTutorialAfterDelay());
    }

    IEnumerator StartTutorialAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);
        StartTutorial();
    }

    void StartTutorial()
    {
        tutorialRunning = true;

        if (tutorialSteps.Length > 0)
            tutorialSteps[0].SetActive(true);

        timer = 0f;
        teleportTimer = 0f;
    }

    void Update()
    {
        if (!tutorialRunning) return;

        if (avanzarAutomaticamente)
        {
            timer += Time.deltaTime;
            if (timer >= autoNextDelay)
            {
                NextStep();
                timer = 0f;
            }
        }
        else
        {
            CheckForActions();
        }
    }

    void CheckForActions()
    {
        if (currentStep >= tutorialSteps.Length) return;

        string stepName = tutorialSteps[currentStep].name.ToLower();

        // --- Movimiento ---
        if (moveAction && moveAction.action.ReadValue<Vector2>().magnitude > 0.3f && stepName.Contains("move"))
            NextStep();

        // --- Agarrar (Grip lateral) ---
        if (gripAction && gripAction.action.WasPressedThisFrame() && stepName.Contains("grab"))
            NextStep();

        // --- Teletransporte (mantener y soltar gatillo) ---
        if (triggerAction && stepName.Contains("teleport"))
        {
            float triggerValue = triggerAction.action.ReadValue<float>();

            if (triggerValue > 0.8f) // se empieza a mantener
            {
                triggerHeld = true;
                teleportTimer += Time.deltaTime;
            }
            else
            {
                if (triggerHeld && teleportTimer >= teleportHoldTime)
                {
                    // soltó el gatillo después de mantenerlo el tiempo suficiente
                    NextStep();
                }
                // reset
                triggerHeld = false;
                teleportTimer = 0f;
            }
        }
    }

    public void NextStep()
    {
        if (currentStep < tutorialSteps.Length - 1)
        {
            tutorialSteps[currentStep].SetActive(false);
            currentStep++;
            tutorialSteps[currentStep].SetActive(true);
        }
        else
        {
            EndTutorial();
        }
    }

    void EndTutorial()
    {
        tutorialRunning = false;

        if (currentStep < tutorialSteps.Length)
            tutorialSteps[currentStep].SetActive(false);

        if (objectToActivateAtEnd != null)
            leftControllerModel.SetActive(false);
            rightControllerModel.SetActive(false);
            objectToActivateAtEnd.SetActive(true);

        gameObject.SetActive(false);
    }

    void EnableInputs()
    {
        if (gripAction) gripAction.action.Enable();
        if (triggerAction) triggerAction.action.Enable();
        if (moveAction) moveAction.action.Enable();
    }

    void OnDisable()
    {
        if (gripAction) gripAction.action.Disable();
        if (triggerAction) triggerAction.action.Disable();
        if (moveAction) moveAction.action.Disable();
    }
}
