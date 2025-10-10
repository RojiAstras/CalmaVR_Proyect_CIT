using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionnaireManager : MonoBehaviour
{

    [Header("UI Elements")]
    public GameObject questionnairePanel;   // Panel principal del cuestionario
    public TMP_Text questionText;
    public Slider scaleSlider; //para preguntas de "1 a 10"
    public Button yesButton;
    public Button noButton;
    public Button nextButton;

    public GameObject ClosedPanel;

    [Header("Preguntas")]
    public List<Question> questionList = new List<Question>();

    private int currentQuestionIndex = 0;
    private QuestionnaireData currentData;

    // Start is called before the first frame update
    void Start()
    {
        //questionnairePanel.SetActive(false);
        //namePanel.SetActive(true);
        //startButton.onClick.AddListener(OnStartClicked);
        currentData = new QuestionnaireData()
        {
            QuestionnaireId = Guid.NewGuid().ToString(),
            userName = "Anonimo",
            dateTime = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss")
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

        questionText.text = "Gracias por tu Opinion";
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
    }
}
