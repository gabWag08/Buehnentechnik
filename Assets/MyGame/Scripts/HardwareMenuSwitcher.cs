using UnityEngine;
using UnityEngine.XR;

public class HardwareMenuSwitcher : MonoBehaviour
{
    [Header("Pages")]
    public GameObject hardwarePage1;
    public GameObject hardwarePage2;

    private bool showingFirstPage = true;

    private InputDevice rightHandDevice;

    private bool aPressedLastFrame;

    void Start()
    {
        // Initial page setup
        hardwarePage1.SetActive(false);
        hardwarePage2.SetActive(false);
    }

    void Update()
    {
        // Reconnect controller if invalid
        if (!rightHandDevice.isValid)
        {
            rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }

        // Read A button
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed))
        {
            // Debug
            if (aPressed)
            {
                Debug.Log("A BUTTON PRESSED");
            }

            // Trigger only once
            if (aPressed && !aPressedLastFrame)
            {
                ToggleHardwarePage();
            }

            aPressedLastFrame = aPressed;
        }
    }

    public void ToggleHardwarePage()
    {
        showingFirstPage = !showingFirstPage;

        hardwarePage1.SetActive(showingFirstPage);
        hardwarePage2.SetActive(!showingFirstPage);

        Debug.Log("SWITCHED PAGE");
    }
}