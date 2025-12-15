using UnityEngine;
using UnityEngine.InputSystem; // Abhängig von Ihrem Input-System (Oculus Integration, OpenXR, Unity Input System, etc.)

public class MenuVisibilityController : MonoBehaviour
{
    // *** Inspector-Variablen ***
    [Tooltip("Der Root des Menüs, der ein- und ausgeblendet werden soll.")]
    public GameObject menuRoot;
    [Tooltip("Der Schwellenwert in Grad. Wenn die Controller-Rotation (z.B. nach oben) diesen Wert überschreitet, wird das Menü angezeigt.")]
    public float activationAngleThreshold = 220f;
    
    // Wir nehmen an, der Controller ist an einen Input-Action-Asset gebunden, 
    // das die Rotation in einem geeigneten Format liefert (z.B. eine Quaternion/Vector3).
    // Wenn Sie ein spezifisches SDK wie Oculus/SteamVR verwenden, 
    // kann der Zugriff auf die Rotation anders erfolgen (z.B. OVRInput.GetLocalControllerRotation).
    
    // *** Private Variablen ***
    private bool isMenuVisible = false;

    void Update()
    {
        // 1. Die lokale (oder Welt-) Rotation des Controllers abrufen
        // Wir gehen davon aus, dass die Standard-VR-Controller-Ausrichtung die Z-Achse nach vorne zeigt.
        // Die Rotation, die Sie suchen, ist typischerweise die **X-Achse** (Pitch/Nicken), 
        // da Sie den Controller nach oben drehen, um in Richtung Ihres Gesichts zu schauen.
        
        // Die lokale Rotation in Euler-Winkel konvertieren
        Vector3 currentEulerRotation = transform.localEulerAngles;
        
        // Unity gibt Euler-Winkel zwischen 0 und 360 aus. 
        // Wir müssen sie in den Bereich von -180 bis 180 konvertieren, um den Winkel nach oben (z.B. 60 Grad) korrekt zu messen.
        float pitchAngle = NormalizeAngle(currentEulerRotation.x); 

        // 2. Schwellenwert-Prüfung
        if (pitchAngle > activationAngleThreshold && !isMenuVisible)
        {
            // Menü anzeigen
            menuRoot.SetActive(true);
            isMenuVisible = true;
        }
        else if (pitchAngle < activationAngleThreshold - 10f && isMenuVisible) // -10f für eine kleine Hysterese
        {
            // Menü ausblenden
            menuRoot.SetActive(false);
            isMenuVisible = false;
        }
    }

    // Hilfsfunktion zur Normalisierung von 0-360 Grad auf -180 bis 180 Grad
    float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle > 180)
        {
            return angle - 360;
        }
        return angle;
    }
}