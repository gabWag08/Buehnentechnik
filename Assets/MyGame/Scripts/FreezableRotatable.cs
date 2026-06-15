using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FreezableRotatable : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 120f;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    private bool isFrozen;
    private bool rotateMode;

    private bool aPressedLastFrame;

    private InputDevice rightHandDevice;
    private InputDevice leftHandDevice;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void Update()
    {
        if (!grabInteractable.isSelected)
            return;

        if (!rightHandDevice.isValid)
        {
            rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }

        // ===== A-Button: Freeze / Unfreeze =====
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed))
        {
            if (aPressed && !aPressedLastFrame)
            {
                ToggleFreeze();
            }

            aPressedLastFrame = aPressed;
        }

        // ===== B-Button: Rotate Mode =====
        leftHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out rotateMode);

        // ===== Joystick Rotation =====
        if (rotateMode)
        {
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
            {
                transform.Rotate(
                    Vector3.up,
                    axis.x * rotationSpeed * Time.deltaTime,
                    Space.World);
            }
        }
    }

    void ToggleFreeze()
    {
        isFrozen = !isFrozen;

        if (isFrozen)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;

            // Optional: nicht mehr greifbar
            // grabInteractable.enabled = false;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;

            // Optional:
            // grabInteractable.enabled = true;
        }

        Debug.Log($"{gameObject.name} Frozen: {isFrozen}");
    }
}