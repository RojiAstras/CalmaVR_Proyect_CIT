using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VirtualKeyboard : MonoBehaviour
{
    [Header("Asignaciones")]
    public TMP_InputField targetInputField;
    public GameObject keyboardCanvas;

    private string currentText = "";

    // Start is called before the first frame update
    void Start()
    {
        keyboardCanvas.SetActive(false);
    }

    public void OpenKeyboard(TMP_InputField input)
    {
        targetInputField = input;
        currentText = input.text;
        keyboardCanvas.SetActive(true);
    }

    public void CloseKeyboard()
    {
        keyboardCanvas.SetActive(false);
        if (targetInputField != null)
        {
            targetInputField.text = currentText;
        }
        targetInputField = null;

    }

    public void AddCharacter(string character)
    {
        currentText += character;
        UpdateDisplay();
    }

    public void BackSpace()
    {
        if (currentText.Length > 0)
        {
            currentText = currentText.Substring(0, currentText.Length - 1);
            UpdateDisplay();
        }
    }

    public void AddSpace()
    {
        currentText += " ";
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if(targetInputField != null)
        {
            targetInputField.text = currentText;
        }
    }

    public void Confirm()
    {
        CloseKeyboard();
    }
}
