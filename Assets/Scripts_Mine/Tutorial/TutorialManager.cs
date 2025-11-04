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
    [Tooltip("Grabaciones de voz para cada paso (mismo orden que tutorialSteps)")]
    public AudioClip[] stepVoiceClips;

    [Header("Voz final del tutorial")]
    public AudioClip finalVoiceClip;

    [Header("Audio Source")]
    [Tooltip("Fuente de audio que reproducirá las voces del tutorial")]
    public AudioSource audioSource;

    [Header("Acciones de entrada")]
    public InputActionReference gripAction;
    public InputActionReference triggerAction;
    public InputActionReference moveAction;

    [Header("Configuración general")]
    public bool avanzarAutomaticamente = false;
    [Tooltip("Segundos extra que se agregan al final de cada grabación antes de pasar al siguiente paso")]
    public float extraDelayPorPaso = 2f;
    public float teleportHoldTime = 1.0f;

    [Header("Inicio del tutorial")]
    public float startDelay = 2f;

    [Header("Objeto a activar al finalizar el tutorial")]
    public GameObject objectToActivateAtEnd;

    private int currentStep = 0;
    private bool tutorialRunning = false;
    private float teleportTimer = 0f;
    private bool triggerHeld = false;

    void Start()
    {
        foreach (var step in tutorialSteps)
            step.SetActive(false);

        EnableInputs();
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
        {
            tutorialSteps[0].SetActive(true);
            PlayVoiceForStep(0);
        }
    }

    void Update()
    {
        if (!tutorialRunning) return;

        if (!avanzarAutomaticamente)
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

            if (triggerValue > 0.8f)
            {
                triggerHeld = true;
                teleportTimer += Time.deltaTime;
            }
            else
            {
                if (triggerHeld && teleportTimer >= teleportHoldTime)
                    NextStep();

                triggerHeld = false;
                teleportTimer = 0f;
            }
        }
    }

    void PlayVoiceForStep(int stepIndex)
    {
        if (audioSource && stepVoiceClips != null && stepIndex < stepVoiceClips.Length && stepVoiceClips[stepIndex] != null)
        {
            audioSource.Stop();
            audioSource.clip = stepVoiceClips[stepIndex];
            audioSource.Play();

            if (avanzarAutomaticamente)
                StartCoroutine(AutoNextAfterClip(stepVoiceClips[stepIndex].length + extraDelayPorPaso));
        }
        else if (avanzarAutomaticamente)
        {
            // Si no hay clip, usar solo el retraso extra
            StartCoroutine(AutoNextAfterClip(extraDelayPorPaso));
        }
    }

    IEnumerator AutoNextAfterClip(float delay)
    {
        yield return new WaitForSeconds(delay);
        NextStep();
    }

    public void NextStep()
    {
        if (currentStep < tutorialSteps.Length - 1)
        {
            tutorialSteps[currentStep].SetActive(false);
            currentStep++;
            tutorialSteps[currentStep].SetActive(true);
            PlayVoiceForStep(currentStep);
        }
        else
        {
            StartCoroutine(EndTutorialWithVoice());
        }
    }

    IEnumerator EndTutorialWithVoice()
    {
        tutorialRunning = false;

        if (currentStep < tutorialSteps.Length)
            tutorialSteps[currentStep].SetActive(false);

        // Reproducir la voz final (si existe)
        if (audioSource && finalVoiceClip != null)
        {
            audioSource.Stop();
            audioSource.clip = finalVoiceClip;
            audioSource.Play();
            yield return new WaitForSeconds(finalVoiceClip.length + extraDelayPorPaso);
        }

        if (objectToActivateAtEnd != null)
        {
            leftControllerModel.SetActive(false);
            rightControllerModel.SetActive(false);
            objectToActivateAtEnd.SetActive(true);
        }

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
