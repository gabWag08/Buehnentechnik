using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MuicChanger : MonoBehaviour
{
    public Button button;
    public Button button2;

    public AudioClip audioClip;
    public AudioClip audioClip2;
    public VideoClip videoclip;
    public VideoClip videoclip2;

    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioSource audioSource3;
    public AudioSource audioSource4;
    //VideoPlayer
    public VideoPlayer videoPlayer;

    public void ChangeSong1()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        audioSource2.clip = audioClip;
        audioSource2.Play();
        audioSource3.clip = audioClip;
        audioSource3.Play();
        audioSource4.clip = audioClip;
        audioSource4.Play();
    
        videoPlayer.clip = videoclip;
        videoPlayer.Play();
    
    }

    public void ChangeSong2()
    {
        audioSource.clip = audioClip2;
        audioSource.Play();
        audioSource2.clip = audioClip2;
        audioSource2.Play();
        audioSource3.clip = audioClip2;
        audioSource3.Play();
        audioSource4.clip = audioClip2;
        audioSource4.Play();

        videoPlayer.clip = videoclip2;
        videoPlayer.Play();
    }
    
}
