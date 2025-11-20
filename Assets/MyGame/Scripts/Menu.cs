using UnityEngine;
using TMPro;

public class Inputviewer : MonoBehaviour
{
    public GameObject panel;
    public Canvas canvas;

    // WICHTIG: TMP_InputField, nicht TextMeshProUGUI!
    public TMP_InputField Length;
    public TMP_InputField Width;
    public TMP_InputField Height;

    public GameObject roomPrefab;

    private int scaleMod = 100;

    public void MakeVisible()
    {
        panel.SetActive(true);
    }

    public void CreateRoom()
    {
        // Zahlen aus den InputFields lesen
        int length, width, height;

        if (!int.TryParse(Length.text, out length) ||
            !int.TryParse(Width.text, out width) ||
            !int.TryParse(Height.text, out height))
        {
            Debug.LogError("Bitte gültige Zahlen eingeben!");
            return;
        }

        // Canvas verstecken
        canvas.gameObject.SetActive(false);

        // Raum erstellen
        GameObject roomInstance = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);

        // Raum skalieren
        roomInstance.transform.localScale = new Vector3(length*scaleMod, height*scaleMod, width*scaleMod);
    }
}
