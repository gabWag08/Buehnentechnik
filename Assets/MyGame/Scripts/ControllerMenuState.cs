using UnityEngine;

public class ControllerMenuState : MonoBehaviour
{
    public GameObject detailHardware1;
    public GameObject detailHardware2;
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
        detailHardware1.SetActive(false);
        detailHardware2.SetActive(false);
        detailMaterials.SetActive(false);
        objectOverviewParent.SetActive(false);
        objectOverviewDetail.SetActive(false);
        objectOverviewDeleteAll.SetActive(false);
        xrScalling.SetActive(false);
    }

    public void ShowHardware()
    {
        detailHardware1.SetActive(true);
        detailHardware2.SetActive(false);
        detailMaterials.SetActive(false);
        objectOverviewParent.SetActive(false);
        objectOverviewDetail.SetActive(false);
        objectOverviewDeleteAll.SetActive(false);
        xrScalling.SetActive(false);
    }

    public void ShowMaterials()
    {
        detailMaterials.SetActive(true);
        detailHardware1.SetActive(false);
        detailHardware2.SetActive(false);
        objectOverviewParent.SetActive(false);
        objectOverviewDetail.SetActive(false);
        objectOverviewDeleteAll.SetActive(false);
        xrScalling.SetActive(false);
    }
    
    public void ShowObjectOverview()
    {
        objectOverviewParent.SetActive(true);
        detailHardware1.SetActive(false);
        detailHardware2.SetActive(false);
        detailMaterials.SetActive(false);
        objectOverviewDeleteAll.SetActive(true);
        xrScalling.SetActive(false);
    }

    public void ShowXRScalling()
    {
        xrScalling.SetActive(true);
        detailHardware1.SetActive(false);
        detailHardware2.SetActive(false);
        detailMaterials.SetActive(false);
        objectOverviewParent.SetActive(false);
        objectOverviewDetail.SetActive(false);
        objectOverviewDeleteAll.SetActive(false);
    }
}
