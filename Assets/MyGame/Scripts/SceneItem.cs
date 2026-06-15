using UnityEngine;

public class SceneItem : MonoBehaviour
{
    [Header("Optional")]
    public AudioSource audioSource;

    [Header("Required")]
    public string itemType; // "Speaker", "Microphone", etc.
    public string displayName; // "Speaker1", "Speaker2"


    private void OnEnable()
{
    if (SceneItemManager.Instance != null)
    {
        Debug.Log("Registering: " + gameObject.name);
        SceneItemManager.Instance.Register(this);
    }
}

    private void OnDisable()
    {
        if (SceneItemManager.Instance != null)
            SceneItemManager.Instance.Unregister(this);
    }
}