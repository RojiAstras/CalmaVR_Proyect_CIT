using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneEntryFade : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;
    public float delayBeforeFade = 0f;

    void Start()
    {
        if (fadeCanvasGroup != null)
        {
            // Inicia en negro
            fadeCanvasGroup.alpha = 1f;
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        // Espera opcional antes de comenzar el fade (por ejemplo, 0.5s)
        if (delayBeforeFade > 0f)
            yield return new WaitForSeconds(delayBeforeFade);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;

        // Opcional: Desactivar el canvas cuando el fade termina
        fadeCanvasGroup.gameObject.SetActive(false);
    }
}
