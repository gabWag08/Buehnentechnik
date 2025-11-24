using UnityEngine;

public class ObjSpawner : MonoBehaviour
{
    public Transform spawnPoint; // z.B. vor der Kamera
    public GameObject currentPrefab;

    public void SetPrefab(GameObject prefab)
    {
        currentPrefab = prefab;
    }

    public void SpawnObject()
    {
        if (currentPrefab == null) return;

        GameObject newObj = Instantiate(currentPrefab, spawnPoint.position, spawnPoint.rotation);

        // XR Grab Interaction hinzufügen, damit man das Objekt greifen kann
        if (!newObj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>())
            newObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }
}
