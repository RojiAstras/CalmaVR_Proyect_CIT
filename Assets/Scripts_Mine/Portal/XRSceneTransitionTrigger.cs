using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class XRSceneTransitionTrigger : MonoBehaviour
{
    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup; // El CanvasGroup con la imagen negra para el fade
    public float fadeDuration = 1f;

    [Header("Target Scene")]
    public string sceneToLoad;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartSceneTransition();
        }
    }

    public void StartSceneTransition()
    {
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {

        // Asegurar que el fade canvas esté activo
        fadeCanvasGroup.gameObject.SetActive(true);

        // Fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // Esperar un momento en negro (opcional)
        yield return new WaitForSeconds(0.5f);

        // Cargar escena
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }

        fadeCanvasGroup.alpha = endAlpha;
    }
}
