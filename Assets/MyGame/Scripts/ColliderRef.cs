using UnityEngine;

public class ColliderRef : MonoBehaviour
{
    public Transform Head;
    public Transform FloorReference;

    CapsuleCollider capsuleCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        float height = Head.position.y - FloorReference.position.y;
        capsuleCollider.height = height;
        transform.position = Head.position - Vector3.up * (height / 2);
    }
}
