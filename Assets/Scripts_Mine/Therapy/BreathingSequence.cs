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
    public TMP_Text phaseText; //Texto "Inhala / Manten / Exhala"

    [Header("Settings")]
    public int totalCycles = 3;
    public float inhaleTime = 4f;
    public float holdTime = 7f;
    public float exhaleTime = 8f;
    public float minScale = 0.5f;
    public float maxScale = 1.0f;

    [Header("Colors por fase")]
    public Color inhaleColor = new Color(0.3f, 0.6f, 1f); //Azul suave
    public Color holdColor = Color.white;
    public Color exhaleColor = new Color(0.2f, 0.4f, 0.8f); //Azul Oscuro
    

    [Header("Intro Settings")]
    public float introDuration = 5f;

    private bool breathingActive = false;
    private Renderer sphereRenderer;
    private int currentCycle = 0;

    private void Start()
    {
        if (breathingSphere != null)
        {
            sphereRenderer = breathingSphere.GetComponent<Renderer>();
        }

        if (phaseText != null)
        {
            phaseText.text = "";
        }
    }

    public void StartSequence ()
    {
        panelStart.SetActive(false);
        StartCoroutine(ShowIntroThenBreathing());
    }

    private IEnumerator ShowIntroThenBreathing()
    {
        panelintro.SetActive(true);
        yield return new WaitForSeconds(introDuration);
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

            //Inhalar (Aumenta el tamaño)
            if (phaseText) phaseText.text = "Inhala";
            yield return StartCoroutine(ScaleSphere(maxScale, inhaleTime, inhaleColor));

            // Mantener (aguante)
            if (phaseText) phaseText.text = "Manten";
            yield return StartCoroutine(HoldPhase(holdTime, holdColor));

            //Exhalar (disminuye el tamaño)
            if (phaseText) phaseText.text = "Exhala";
            yield return StartCoroutine(ScaleSphere(minScale, exhaleTime, exhaleColor));
        }

        //Final
        breathingActive = false;
        if (phaseText) phaseText.text = "Ejercicio Completado";
        yield return new WaitForSeconds(3f);
        phaseText.text = "";
        breathingSphere.SetActive(false);
        panelStart.SetActive(true); // volver al inicio o tambien cambiarlo a una pantalla que diga "ejercicio completado"
    }

    private IEnumerator ScaleSphere(float targetScale, float duration, Color targetColor)
    {
        Vector3 startScake = breathingSphere.transform.localScale;
        Vector3 endScale = Vector3.one * targetScale;
        Color startColor = sphereRenderer.material.color;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            breathingSphere.transform.localScale = Vector3.Lerp(startScake, endScale, t);
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

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            sphereRenderer.material.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        sphereRenderer.material.color = targetColor;
    }
}
