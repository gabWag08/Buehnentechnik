using SteamAudio;
using UnityEngine;


[CreateAssetMenu(menuName = "Audio/SteamAudioMaterial")]
public class SteamAudioMaterialData : ScriptableObject
{
    public string materialName;
    public Color previewColor;

    public SteamAudioMaterial steamAudioMaterial;
}