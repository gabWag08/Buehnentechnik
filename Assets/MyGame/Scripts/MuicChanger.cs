using UnityEngine;
using UnityEngine.UI;
public class MuicChanger : MonoBehaviour
{
    public Button button;
    public Button button2;

    public AudioClip audioClip;
    public AudioClip audioClip2;

    public AudioSource audioSource;

    public void ChangeSong1()
    {
        //Bei Button1 soll in die AudioSource der AudioClip1 geladen und abgespielt werden
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void ChangeSong2()
    {
        audioSource.clip = audioClip2;
        audioSource.Play();
    }
    
}
