using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ObjSpawner : MonoBehaviour
{
    public GameObject currentPrefab;
    public NearFarInteractor handInteractor;


    public void SpawnObject(GameObject prefab) 
    {
        currentPrefab = prefab;
        if (currentPrefab == null)
        {
            Debug.LogWarning("No prefab selected!");
            return;
        }

        // Spawn slightly in front of the hand
        Vector3 spawnPos = handInteractor.transform.position + handInteractor.transform.forward * 0.1f;

        GameObject obj = Instantiate(currentPrefab, spawnPos, Quaternion.identity);

        XRGrabInteractable grab = obj.GetComponent<XRGrabInteractable>();

        if (grab != null)
        {
            handInteractor.interactionManager.SelectEnter(
                (IXRSelectInteractor)handInteractor,
                (IXRSelectInteractable)grab
            );
        }
        else
        {
            Debug.LogWarning("Spawned object has no XRGrabInteractable!");
        }
    }
}
