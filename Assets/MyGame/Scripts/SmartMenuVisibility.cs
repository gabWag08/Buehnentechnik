using UnityEngine;

public class SmartMenuVisibility : MonoBehaviour
{
    [Header("Wichtige Zuweisungen")]
    public Transform headCamera;
    public GameObject menuRoot;
    
    [Tooltip("Ziehe hier das 'MenuAimReference' Objekt rein")]
    public Transform aimReference; 

    [Header("Feinjustierung")]
    [Range(0f, 1f)]
    public float activationThreshold = 0.7f; // Etwas toleranter eingestellt (0.7 ist oft besser als 0.85)
    public bool showDebugLines = true;

    void Start()
    {
        if (headCamera == null) headCamera = Camera.main.transform;
        if(menuRoot != null) menuRoot.SetActive(false);
    }

    void Update()
    {
        if (headCamera == null || menuRoot == null || aimReference == null) return;

        CheckMenuVisibility();
    }

    void CheckMenuVisibility()
    {
        // Richtung zum Kopf
        Vector3 directionToHead = (headCamera.position - transform.position).normalized;

        // WICHTIG: Wir nutzen jetzt die 'forward' (Blauer Pfeil) Richtung deiner Reference!
        Vector3 pointingDirection = aimReference.forward; 

        float dotProduct = Vector3.Dot(pointingDirection, directionToHead);

        if (dotProduct > activationThreshold)
        {
            if (!menuRoot.activeSelf) menuRoot.SetActive(true);
        }
        else
        {
            if (menuRoot.activeSelf) menuRoot.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugLines || aimReference == null) return;

        // Zeigt jetzt den Blauen Pfeil deiner Reference an
        Gizmos.color = Color.green; // Zeichne es trotzdem grün, damit man es gut sieht
        Gizmos.DrawRay(aimReference.position, aimReference.forward * 0.5f);

        if (headCamera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, headCamera.position);
        }
    }
}