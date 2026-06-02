using SteamAudio;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MaterialApplicator : MonoBehaviour
{
    [Header("Current Material")]
    public SteamAudioMaterialData currentMaterial;

    [Header("References")]
    public Transform controller;
    public NearFarInteractor rightInteractor;
    public InputActionReference triggerAction;


    [Header("Raycast")]
    public LayerMask applyLayer;
    public float rayDistance = 10f;

    [Header("Visual Feedback")]
    public Renderer previewSphere;

    [Header("Gun State")]
    public bool isGunActive = true;

    private Renderer highlightedRenderer;
    private Color originalColor;

    // Trigger detection
    private bool wasSelectingLastFrame;

    void Start()
    {
        // Hide preview sphere at start
        if (previewSphere != null)
        {
            previewSphere.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isGunActive)
        {
            ClearHighlight();
            wasSelectingLastFrame = false;
            return;
        }
        // No material selected → do nothing
        if (currentMaterial == null)
        {
            ClearHighlight();
            return;
        }

        // Create ray from controller forward
        UnityEngine.Ray ray = new UnityEngine.Ray(controller.position, controller.forward);

        // Debug Ray in Scene View
        Debug.DrawRay(controller.position, controller.forward * rayDistance, Color.red);

        // Check if ray hits valid object
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, applyLayer))
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();

            if (rend != null)
            {
                Highlight(rend);

                // Check trigger press
                bool isSelecting = triggerAction.action.IsPressed();

                // Trigger pressed THIS frame
                if (isSelecting && !wasSelectingLastFrame)
                {
                    Debug.Log("TRIGGER PRESSED");

                    ApplyMaterial(hit.collider.gameObject);
                }

                // Save previous trigger state
                wasSelectingLastFrame = isSelecting;
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    // =========================
    // MATERIAL SELECTION
    // =========================
    public void SelectMaterial(SteamAudioMaterialData mat)
    {
        currentMaterial = mat;

        Debug.Log("Selected Material: " + mat.materialName);

        // Show preview sphere
        if (previewSphere != null)
        {
            previewSphere.gameObject.SetActive(true);
            previewSphere.material.color = mat.previewColor;
        }
    }

    // =========================
    // APPLY MATERIAL
    // =========================
    void ApplyMaterial(GameObject obj)
    {
        SteamAudioGeometry geo = obj.GetComponent<SteamAudioGeometry>();

        if (geo != null)
        {
            geo.material = currentMaterial.steamAudioMaterial;

            Debug.Log("Applied " + currentMaterial.materialName + " to " + obj.name);

            // TEMP visual feedback
            Renderer rend = obj.GetComponent<Renderer>();

            if (rend != null)
            {
                rend.material.color = currentMaterial.previewColor;
            }
            previewSphere.gameObject.SetActive(false);
            SetGunActive(false);
        }
        else
        {
            Debug.LogWarning("No SteamAudioGeometry found on: " + obj.name);
        }
    }

    // =========================
    // HIGHLIGHT
    // =========================
    void Highlight(Renderer rend)
    {
        // Already highlighted
        if (highlightedRenderer == rend)
            return;

        // Remove old highlight
        ClearHighlight();

        highlightedRenderer = rend;

        originalColor = rend.material.color;

        rend.material.color = currentMaterial.previewColor;
    }

    void ClearHighlight()
    {
        if (highlightedRenderer != null)
        {
            highlightedRenderer.material.color = originalColor;
            highlightedRenderer = null;
        }
    }

    public void SetGunActive(bool active)
    {
        isGunActive = active;

        if (!active)
        {
            ClearCurrentMaterial();
            ClearHighlight();
        }

        Debug.Log("Material Gun Active: " + active);
    }

    // =========================
    // EXIT MATERIAL MODE
    // =========================
    public void ClearCurrentMaterial()
    {
        currentMaterial = null;

        ClearHighlight();

        if (previewSphere != null)
        {
            previewSphere.gameObject.SetActive(false);
        }
    }
}