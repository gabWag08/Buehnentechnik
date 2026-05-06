using TMPro;
using UnityEngine;

public class VRNumpad : MonoBehaviour
{
    public TMP_InputField currentField;

    public GameObject Numpad;


    private void Start() =>
    Numpad.SetActive(false);

    public void AddDigit(string digit)
    {
        if (currentField == null) return;

        // Prevent multiple dots
        if (digit == "." && currentField.text.Contains(".")) return;

        currentField.text += digit;
    }

    public void Backspace()
    {
        if (currentField == null) return;

        if (currentField.text.Length > 0)
        {
            currentField.text = currentField.text.Substring(0, currentField.text.Length - 1);
        }
    }

    public void Clear()
    {
        if (currentField == null) return;

        currentField.text = "";
    }

    public float GetValue()
    {
        if (currentField == null) return 0;

        float.TryParse(currentField.text, out float value);
        return value;
    }

    public void Confirm()
    {
        Debug.Log("Height: " + GetValue());
    }

    public void SetActiveField(TMP_InputField field)
    {
        currentField = field;
        Debug.Log("Active Field: " + field.name);

        Numpad.SetActive(true);

        field.image.color = Color.yellow;
    }
}