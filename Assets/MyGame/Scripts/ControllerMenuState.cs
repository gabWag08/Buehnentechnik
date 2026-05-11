using UnityEngine;

public class ControllerMenuState : MonoBehaviour
{
    public GameObject detailHardware;
    public GameObject detailMaterials;
    public GameObject objectOverviewParent;
    public GameObject objectOverviewDetail;
    public GameObject objectOverviewDeleteAll;
    public GameObject xrScalling;

    void OnEnable()
    {
        ResetMenu();
    }

    public void ResetMenu()
    {
        detailHardware.SetActive(false);
        detailMaterials.SetActive(false);
        objectOverviewParent.SetActive(false);
        objectOverviewDetail.SetActive(false);
        objectOverviewDeleteAll.SetActive(false);
        xrScalling.SetActive(false);
    }

    public void ShowHardware()
    {
        detailHardware.SetActive(true);
        detailMaterials.SetActive(false);
        objectOverviewParent.SetActive(false);
        objectOverviewDetail.SetActive(false);
        objectOverviewDeleteAll.SetActive(false);
        xrScalling.SetActive(false);
    }

    public void ShowMaterials()
    {
        detailMaterials.SetActive(true);
        detailHardware.SetActive(false);
        objectOverviewParent.SetActive(false);
        objectOverviewDetail.SetActive(false);
        objectOverviewDeleteAll.SetActive(false);
        xrScalling.SetActive(false);
    }
    
    public void ShowObjectOverview()
    {
        objectOverviewParent.SetActive(true);
        detailHardware.SetActive(false);
        detailMaterials.SetActive(false);
        objectOverviewDeleteAll.SetActive(true);
        xrScalling.SetActive(false);
    }

    public void ShowXRScalling()
    {
        xrScalling.SetActive(true);
        detailHardware.SetActive(false);
        detailMaterials.SetActive(false);
        objectOverviewParent.SetActive(false);
        objectOverviewDetail.SetActive(false);
        objectOverviewDeleteAll.SetActive(false);
    }
}
