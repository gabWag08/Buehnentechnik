using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VRNumpad : MonoBehaviour
{
    public TMP_InputField inputField;

    // Input Displays
    public InputField height;
    public InputField width;
    public InputField length;

    public TMP_InputField currentField;

    public void AddDigit(string digit)
    {
        inputField.text += digit;
    }

    public void Backspace()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }

    public void Clear()
    {
        inputField.text = "";
    }

    public float GetValue()
    {
        float.TryParse(inputField.text, out float value);
        return value;
    }

    public void Confirm()
    {
        float value = GetValue();
        Debug.Log("Entered Value: " + value);

    }

    public void SetActiveField(TMP_InputField field)
    {
        currentField = field;
    }
}
