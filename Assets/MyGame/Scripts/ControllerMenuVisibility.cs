using UnityEngine;

public class ControllerMenuVisibility : MonoBehaviour
{
    public GameObject menuCanvas;
    public Transform head;

    // Which direction the menu faces (adjust if needed)
    public Vector3 localMenuNormal = Vector3.right;

    public float showAngle = 60f; // degrees

    void Start()
    {
        // Start hidden
        menuCanvas.SetActive(false);
    }

    void Update()
    {
        // Convert local direction to world direction
        Vector3 menuDirection = transform.TransformDirection(localMenuNormal);

        // Direction from controller to head
        Vector3 toHead = (head.position - transform.position).normalized;

        float angle = Vector3.Angle(menuDirection, toHead);

        if (angle < showAngle)
        {
            if (!menuCanvas.activeSelf)
                menuCanvas.SetActive(true);
        }
        else
        {
            if (menuCanvas.activeSelf)
                menuCanvas.SetActive(false);
        }
    }
}
