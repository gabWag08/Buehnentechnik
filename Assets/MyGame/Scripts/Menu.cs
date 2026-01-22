using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Inputviewer : MonoBehaviour
{
    public GameObject panel;
    public GameObject InitRoom;

    // WICHTIG: TMP_InputField, nicht TextMeshProUGUI!
    public TMP_InputField Length;
    public TMP_InputField Width;
    public TMP_InputField Height;

    public GameObject roomPrefab;
    public GameObject XR;
    public Camera cam;
    public Button Createbutton;
    public Button Prebutton;
    public GameObject PreBuildRoom;

    public Button Reload;
    public Button SceneChange;

    private int scaleMod = 100;

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
        InitRoom.gameObject.SetActive(false);
        XR.SetActive(true);
        Destroy(cam.gameObject);

        // Raum erstellen
        GameObject roomInstance = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);

        // Raum skalieren
        roomInstance.transform.localScale = new Vector3(length*scaleMod, height*scaleMod, width*scaleMod);

        MeshCollider col = roomInstance.AddComponent<MeshCollider>();

    
    }

    public void CreatePreBuildRoom()
    {
        InitRoom.gameObject.SetActive(false);
        XR.SetActive(true);
        Destroy(cam.gameObject);
        PreBuildRoom.SetActive(true);

    }

    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void ChangeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("RoomScene");
    }
}
