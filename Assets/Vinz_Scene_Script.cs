using UnityEngine;
using UnityEngine.UI;

public class Vinz_Scene_Script : MonoBehaviour
{
    public Button button;

    public void ChangeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Demo");
    }
}
