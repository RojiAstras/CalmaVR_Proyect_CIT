using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class QuestionnaireManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject questionnairePanel;
    public TMP_Text questionText;
    public Slider scaleSlider;
    public Button yesButton;
    public Button noButton;
    public Button nextButton;

    [Header("Paneles Salida")]
    public GameObject ClosedPanel;
    public GameObject ExitPanel;

    [Header("Preguntas")]
    public List<Question> questionList = new List<Question>();

    [Header("Emoji Settings")]
    [Tooltip("RawImage donde se mostrará el emoji asociado al valor del slider")]
    public RawImage emojiImage; // 👈 cambiado a RawImage
    [Tooltip("Lista de texturas (0 = feliz, 10 = enojado)")]
    public List<Texture2D> emojiTextures = new List<Texture2D>(); // 👈 cambiamos Sprite → Texture2D

    [Header("Audio Final")]
    public AudioClip endAudioClip;
    public AudioSource audioSource;
    public float endAudioDelay = 0.5f;

    private int currentQuestionIndex = 0;
    private QuestionnaireData currentData;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        if (emojiImage != null)
            emojiImage.gameObject.SetActive(false);

        currentData = new QuestionnaireData()
        {
            QuestionnaireId = Guid.NewGuid().ToString(),
            userName = "Anonimo",
            dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name // ✅ Guarda el nombre de la escena
        };


        questionnairePanel.SetActive(true);
        ShowQuestion();

        scaleSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void ShowQuestion()
    {
        if (currentQuestionIndex >= questionList.Count)
        {
            EndQuestionnaire();
            return;
        }

        Question current = questionList[currentQuestionIndex];
        questionText.text = current.questionText;

        bool isScale = current.questionType == "scale";
        scaleSlider.gameObject.SetActive(isScale);
        yesButton.gameObject.SetActive(current.questionType == "yesno");
        noButton.gameObject.SetActive(current.questionType == "yesno");
        nextButton.gameObject.SetActive(isScale);

        if (emojiImage != null)
        {
            bool usarEmojis = current.useEmojis && isScale;
            emojiImage.gameObject.SetActive(usarEmojis);
            if (usarEmojis)
                UpdateEmojiDisplay(Mathf.RoundToInt(scaleSlider.value));
        }
    }

    public void OnSliderValueChanged(float value)
    {
        if (currentQuestionIndex < questionList.Count)
        {
            Question current = questionList[currentQuestionIndex];
            if (current.useEmojis && emojiImage != null)
                UpdateEmojiDisplay(Mathf.RoundToInt(value));
        }
    }

    void UpdateEmojiDisplay(int value)
    {
        if (emojiTextures == null || emojiTextures.Count == 0) return;

        value = Mathf.Clamp(value, 0, Mathf.Min(emojiTextures.Count - 1, 10));
        emojiImage.texture = emojiTextures[value]; // 👈 cambiamos sprite → texture
    }

    // Métodos de UI
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

        questionText.text = "Gracias por tu opinión 😊";
        scaleSlider.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);

        if (emojiImage != null)
            emojiImage.gameObject.SetActive(false);

        Invoke(nameof(DisableCanvas), 2f);
    }

    void DisableCanvas()
    {
        ClosedPanel.SetActive(false);
        ExitPanel.SetActive(true);

        if (endAudioClip != null)
            StartCoroutine(PlayEndAudioAfterDelay());
    }

    IEnumerator PlayEndAudioAfterDelay()
    {
        yield return new WaitForSeconds(endAudioDelay);
        audioSource.clip = endAudioClip;
        audioSource.priority = 10;
        audioSource.Play();
    }
}