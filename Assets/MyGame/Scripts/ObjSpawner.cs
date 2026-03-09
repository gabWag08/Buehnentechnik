using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ObjSpawner : MonoBehaviour
{
    public GameObject currentPrefab;
    public XRDirectInteractor handInteractor;


    public void SetPrefab(GameObject newPrefab)
    {
        currentPrefab = newPrefab;
    }

    public void SpawnObject()
    {
        GameObject obj = Instantiate(currentPrefab, handInteractor.transform.position, Quaternion.identity);

        XRGrabInteractable grab = obj.GetComponent<XRGrabInteractable>();

        handInteractor.interactionManager.SelectEnter(
            (IXRSelectInteractor)handInteractor,
            (IXRSelectInteractable)grab
        );
    }
}
