using UnityEngine;

public class ControllerMenuState : MonoBehaviour
{
    public GameObject detailHardware;
    public GameObject detailMaterials;
    public GameObject objectOverview;
    public GameObject objectOverviewDeleteAll;

    void OnEnable()
    {
        ResetMenu();
    }

    public void ResetMenu()
    {
        detailHardware.SetActive(false);
        detailMaterials.SetActive(false);
        objectOverview.SetActive(false);
        objectOverviewDeleteAll.SetActive(false);
    }

    public void ShowHardware()
    {
        detailHardware.SetActive(true);
        detailMaterials.SetActive(false);
        objectOverview.SetActive(false);
        objectOverviewDeleteAll.SetActive(false);

    }

    public void ShowMaterials()
    {
        detailMaterials.SetActive(true);
        detailHardware.SetActive(false);
        objectOverview.SetActive(false);
        objectOverviewDeleteAll.SetActive(false);
    }
    
    public void ShowObjectOverview()
    {
        objectOverview.SetActive(true);
        detailHardware.SetActive(false);
        detailMaterials.SetActive(false);
        objectOverviewDeleteAll.SetActive(true);
    }
}
