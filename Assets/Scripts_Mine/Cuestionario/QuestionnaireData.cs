using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[System.Serializable]
public class QuestionnaireData
{
    public string QuestionnaireId;
    public string userName;
    public string dateTime;
    public string sceneName;
    public List<Question> questions = new List<Question>();

    // URL de tu webhook (Pipedream, Zapier, etc.)
    private const string webhookUrl = "https://eovjim3zn061lrv.m.pipedream.net";

    public static void SaveToJson(QuestionnaireData data)
    {
        string json = JsonUtility.ToJson(data, true);

        // Guardar localmente en la Quest
        string path = Application.persistentDataPath + $"/Questionnaire_{data.QuestionnaireId}.json";
        File.WriteAllText(path, json);
        Debug.Log("Cuestionario guardado en: " + path);

        // Enviar el JSON al webhook
        var sender = new GameObject("JsonUploader").AddComponent<JsonUploader>();
        sender.SendJson(json, data.QuestionnaireId);
    }

    // Clase auxiliar para manejar el envío (coroutine)
    private class JsonUploader : MonoBehaviour
    {
        public void SendJson(string json, string questionnaireId)
        {
            StartCoroutine(SendCoroutine(json, questionnaireId));
        }

        private IEnumerator SendCoroutine(string json, string questionnaireId)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new UnityWebRequest(webhookUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-Questionnaire-Id", questionnaireId);

            Debug.Log("Enviando cuestionario al webhook...");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                Debug.Log("✅ Cuestionario enviado correctamente.");
            else
                Debug.LogWarning("⚠️ Error al enviar cuestionario: " + request.error);

            Destroy(gameObject); // limpiar objeto temporal
        }
    }
}

