using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Inputviewer : MonoBehaviour
{
    public GameObject panel;
    public GameObject InitRoom;

    public TMP_InputField Length;
    public TMP_InputField Width;
    public TMP_InputField Height;

    public Button Createbutton;
    public Button Prebutton;

    public void MakeVisible()
    {
        Createbutton.gameObject.SetActive(true);
        Prebutton.gameObject.SetActive(true);
    }

    public void PanelVisible()
    {
        panel.SetActive(true);
    }

    public void CreateRoom()
    {
        int length, width, height;

        if (!int.TryParse(Length.text, out length) ||
            !int.TryParse(Width.text, out width) ||
            !int.TryParse(Height.text, out height))
        {
            Debug.LogError("Bitte gültige Zahlen eingeben!");
            return;
        }

        // Daten speichern
        RoomData.length = length;
        RoomData.width = width;
        RoomData.height = height;

        // Szene wechseln (z.B. "RoomScene")
        SceneManager.LoadScene("RoomScene");
    }

    public void CreatePreBuildRoom()
    {
        // Szene "Demo" laden
        SceneManager.LoadScene("Demo");
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("RoomScene");
    }
}