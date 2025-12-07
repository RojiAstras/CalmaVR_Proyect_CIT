using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BreathingSequence : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelStart;
    public GameObject panelintro;
    public GameObject breathingSphere;

    [Header("Texts")]
    public TMP_Text phaseText; // Texto "Inhala / Mantén / Exhala"

    [Header("Settings")]
    public int totalCycles = 3;
    public float inhaleTime = 4f;
    public float holdTime = 7f;
    public float exhaleTime = 8f;
    public float minScale = 0.5f;
    public float maxScale = 1.0f;

    [Header("Colors por fase")]
    public Color inhaleColor = new Color(0.3f, 0.6f, 1f); // Azul suave
    public Color holdColor = Color.white;
    public Color exhaleColor = new Color(0.2f, 0.4f, 0.8f); // Azul oscuro

    [Header("Intro Settings")]
    public AudioClip introAudioClip;
    public float extraIntroDelay = 2f;
    public float introDuration = 5f;

    [Header("Audios por fase")]
    public AudioClip inhaleAudio;
    public AudioClip holdAudio;
    public AudioClip exhaleAudio;

    [Header("Audio Final")]
    public AudioClip endAudioClip;
    public float extraEndDelay = 2f; // tiempo adicional después del audio final

    private bool breathingActive = false;
    private bool breathingStart = false;
    private Renderer sphereRenderer;
    private int currentCycle = 0;
    private AudioSource audioSource;

    public bool IsSequenceRunning { get { return breathingStart; } }
    public bool SequenceFinished { get; private set; } = false;

    private void Start()
    {
        // Buscar o agregar un AudioSource automáticamente
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (breathingSphere != null)
            sphereRenderer = breathingSphere.GetComponent<Renderer>();

        if (phaseText != null)
            phaseText.text = "";
    }

    public void StartSequence()
    {
        panelStart.SetActive(false);
        SequenceFinished = false;
        breathingStart = true;
        StartCoroutine(ShowIntroThenBreathing());
    }

    private IEnumerator ShowIntroThenBreathing()
    {
        panelintro.SetActive(true);

        // Si hay audio de introducción, usar su duración
        if (introAudioClip != null)
        {
            PlayAudio(introAudioClip);
            yield return new WaitForSeconds(introAudioClip.length + extraIntroDelay);
        }
        else
        {
            // Si no hay audio, usar la duración fija
            yield return new WaitForSeconds(introDuration);
        }

        // Ocultar panel e iniciar respiración
        panelintro.SetActive(false);

        breathingSphere.SetActive(true);
        breathingActive = true;
        currentCycle = 0;
        StartCoroutine(BreathingRoutine());
    }

    private IEnumerator BreathingRoutine()
    {
        while (breathingActive && currentCycle < totalCycles)
        {
            currentCycle++;

            // Inhalar
            if (phaseText) phaseText.text = "Inhala";
            PlayAudio(inhaleAudio);
            yield return StartCoroutine(ScaleSphere(maxScale, inhaleTime, inhaleColor));

            // Mantener
            if (phaseText) phaseText.text = "Mantén";
            PlayAudio(holdAudio);
            yield return StartCoroutine(HoldPhase(holdTime, holdColor));

            // Exhalar
            if (phaseText) phaseText.text = "Exhala";
            PlayAudio(exhaleAudio);
            yield return StartCoroutine(ScaleSphere(minScale, exhaleTime, exhaleColor));
        }

        // Final del ejercicio
        breathingStart = false;
        breathingActive = false;
        if (phaseText) phaseText.text = "Ejercicio Completado";


        // Reproducir audio final si existe
        if (endAudioClip != null)
        {
            PlayAudio(endAudioClip);
            yield return new WaitForSeconds(endAudioClip.length + extraEndDelay);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }
        SequenceFinished = true;

        // Limpiar y volver al inicio
        if (phaseText) phaseText.text = "";
        breathingSphere.SetActive(false);
        panelStart.SetActive(true);
    }

    private IEnumerator ScaleSphere(float targetScale, float duration, Color targetColor)
    {
        Vector3 startScale = breathingSphere.transform.localScale;
        Vector3 endScale = Vector3.one * targetScale;
        Color startColor = sphereRenderer.material.color;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            breathingSphere.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            sphereRenderer.material.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        breathingSphere.transform.localScale = endScale;
        sphereRenderer.material.color = targetColor;
    }

    private IEnumerator HoldPhase(float duration, Color targetColor)
    {
        Color startColor = sphereRenderer.material.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            sphereRenderer.material.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        sphereRenderer.material.color = targetColor;
    }

    private void PlayAudio(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }


}
