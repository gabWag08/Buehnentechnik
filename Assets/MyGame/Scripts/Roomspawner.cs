using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public GameObject roomPrefab;
    private int scaleMod = 1;

    void Start()
    {
        if (!PlayerPrefs.HasKey("RoomData"))
        {
            Debug.LogError("Keine gespeicherten Raumdaten gefunden!");
            return;
        }

        string json = PlayerPrefs.GetString("RoomData");

        RoomData data = JsonUtility.FromJson<RoomData>(json);

        GameObject roomInstance = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);

        roomInstance.transform.localScale = new Vector3(
            data.length * scaleMod,
            data.height * scaleMod,
            data.width * scaleMod
        );

        roomInstance.AddComponent<MeshCollider>();
    }
}