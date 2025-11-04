using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // 👈 agregado si usas AudioMixer

public class QuestionnaireManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject questionnairePanel;   // Panel principal del cuestionario
    public TMP_Text questionText;
    public Slider scaleSlider; //para preguntas de "1 a 10"
    public Button yesButton;
    public Button noButton;
    public Button nextButton;

    [Header("Paneles Salida")]
    public GameObject ClosedPanel;
    public GameObject ExitPanel;

    [Header("Preguntas")]
    public List<Question> questionList = new List<Question>();

    [Header("Audio Final")]
    [Tooltip("Clip de audio que se reproducirá al finalizar el cuestionario")]
    public AudioClip endAudioClip;
    [Tooltip("AudioSource que reproducirá el audio final (si no hay uno, se crea automáticamente)")]
    public AudioSource audioSource;
    [Tooltip("Tiempo (en segundos) a esperar después de mostrar el panel final antes de reproducir el audio")]
    public float endAudioDelay = 0.5f;

    private int currentQuestionIndex = 0;
    private QuestionnaireData currentData;

    void Start()
    {
        // Si no hay AudioSource asignado, creamos uno en este objeto
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        currentData = new QuestionnaireData()
        {
            QuestionnaireId = Guid.NewGuid().ToString(),
            userName = "Anonimo",
            dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        questionnairePanel.SetActive(true);
        ShowQuestion();
    }

    void ShowQuestion()
    {
        if (currentQuestionIndex >= questionList.Count)
        {
            //Guardar respuestas cuando termine
            EndQuestionnaire();
            return;
        }

        Question current = questionList[currentQuestionIndex];
        questionText.text = current.questionText;

        scaleSlider.gameObject.SetActive(current.questionType == "scale");
        yesButton.gameObject.SetActive(current.questionType == "yesno");
        noButton.gameObject.SetActive(current.questionType == "yesno");
        nextButton.gameObject.SetActive(current.questionType == "scale");
    }

    // Metodos de UI

    public void OnYesClicked()
    {
        questionList[currentQuestionIndex].yesNoAnswer = true;
        SaveAnswerAndNext();
    }

    public void OnNoClicked()
    {
        questionList[currentQuestionIndex].yesNoAnswer = false;
        SaveAnswerAndNext();
    }

    public void OnNextClicked()
    {
        questionList[currentQuestionIndex].scaleAnswer = Mathf.RoundToInt(scaleSlider.value);
        SaveAnswerAndNext();
    }

    void SaveAnswerAndNext()
    {
        currentData.questions.Add(questionList[currentQuestionIndex]);
        currentQuestionIndex++;
        ShowQuestion();
    }

    void EndQuestionnaire()
    {
        QuestionnaireData.SaveToJson(currentData);

        questionText.text = "Gracias por tu opinión";
        scaleSlider.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);

        // Esperar un par de segundos antes de desactivar el canvas
        Invoke(nameof(DisableCanvas), 2f);
    }

    void DisableCanvas()
    {
        ClosedPanel.SetActive(false);
        ExitPanel.SetActive(true);

        // Reproducir el audio final si existe
        if (endAudioClip != null)
            StartCoroutine(PlayEndAudioAfterDelay());
    }

    IEnumerator PlayEndAudioAfterDelay()
    {
        yield return new WaitForSeconds(endAudioDelay);

        audioSource.clip = endAudioClip;
        audioSource.priority = 10; // Alta prioridad para que no se corte
        audioSource.Play();
    }
}
