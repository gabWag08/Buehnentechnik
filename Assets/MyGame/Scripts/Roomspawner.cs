using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public GameObject roomPrefab;
    private int scaleMod = 1;

    void Start()
    {
        GameObject roomInstance = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);

        roomInstance.transform.localScale = new Vector3(
            RoomData.length * scaleMod,
            RoomData.height * scaleMod,
            RoomData.width * scaleMod
        );

        roomInstance.AddComponent<MeshCollider>();
    }
}