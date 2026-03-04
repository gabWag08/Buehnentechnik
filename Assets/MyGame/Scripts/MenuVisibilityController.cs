using UnityEngine;

public class ControllerMenuVisibility : MonoBehaviour
{
    public GameObject menuCanvas;
    public Transform head;

    [Range(-1f, 1f)]
    public float showThreshold = 0.6f;

    void Update()
    {
        // Change this axis if needed
        Vector3 controllerFacingDirection = -transform.right; 

        Vector3 directionToHead = (head.position - transform.position).normalized;

        float dot = Vector3.Dot(controllerFacingDirection, directionToHead);

        bool shouldShow = dot > showThreshold;

        menuCanvas.SetActive(shouldShow);
    }
}
