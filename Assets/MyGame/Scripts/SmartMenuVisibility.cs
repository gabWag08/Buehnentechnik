using UnityEngine;

public class ControllerMenuState : MonoBehaviour
{
    public GameObject detailHardware;
    public GameObject detailMaterials;

    void OnEnable()
    {
        ResetMenu();
    }

    public void ResetMenu()
    {
        detailHardware.SetActive(false);
        detailMaterials.SetActive(false);
    }

    public void ShowHardware()
    {
        detailHardware.SetActive(true);
        detailMaterials.SetActive(false);
    }

    public void ShowMaterials()
    {
        detailMaterials.SetActive(true);
        detailHardware.SetActive(false);
    }
}
