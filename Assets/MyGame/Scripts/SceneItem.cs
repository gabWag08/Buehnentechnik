using UnityEngine;

public class SceneItem : MonoBehaviour
{
    public string itemType; // "Speaker", "Microphone", etc.
    public string displayName; // "Speaker1", "Speaker2"

    private void OnEnable()
    {
        SceneItemManager.Instance.Register(this);
    }

    private void OnDisable()
    {
        if (SceneItemManager.Instance != null)
            SceneItemManager.Instance.Unregister(this);
    }
}