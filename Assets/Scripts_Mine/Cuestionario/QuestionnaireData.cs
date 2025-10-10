using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class QuestionnaireData
{
    public string QuestionnaireId;
    public string userName;
    public string dateTime;
    public List<Question> questions = new List<Question>();

    public static void SaveToJson(QuestionnaireData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = Application.persistentDataPath + $"/Questionnaire_{data.QuestionnaireId}.json";
        File.WriteAllText(path, json);
        Debug.Log("Cuestionario guardado en: " + path);
    }

}
