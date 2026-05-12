using SteamAudio;
using UnityEngine;

public class MaterialApplicator : MonoBehaviour
{
    public SteamAudioMaterialData currentMaterial;

    public Transform controller;
    public LayerMask applyLayer;

    public Renderer previewSphere;

    private Renderer highlightedRenderer;
    private Color originalColor;

    void Update()
    {
        UnityEngine.Ray ray = new UnityEngine.Ray(controller.position, controller.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f, applyLayer))
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();

            if (rend != null)
            {
                Highlight(rend);

                if (Input.GetButtonDown("Fire1"))
                {
                    ApplyMaterial(hit.collider.gameObject);
                }
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    public void SelectMaterial(SteamAudioMaterialData mat)
    {
        currentMaterial = mat;

        previewSphere.material.color = mat.previewColor;
    }

    void ApplyMaterial(GameObject obj)
    {
        SteamAudioGeometry geo = obj.GetComponent<SteamAudioGeometry>();

        if (geo != null)
        {
            geo.material = currentMaterial.steamAudioMaterial;

            Debug.Log("Applied: " + currentMaterial.materialName);
        }
    }

    void Highlight(Renderer rend)
    {
        if (highlightedRenderer == rend) return;

        ClearHighlight();

        highlightedRenderer = rend;
        originalColor = rend.material.color;

        rend.material.color = Color.green;
    }

    void ClearHighlight()
    {
        if (highlightedRenderer != null)
        {
            highlightedRenderer.material.color = originalColor;
            highlightedRenderer = null;
        }
    }
}