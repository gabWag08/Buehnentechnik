using UnityEngine;
using UnityEngine.Video;

public class PlayPauseMusic : MonoBehaviour
{
    private AudioSource[] speakers;
    private bool isplaying = true;

    [SerializeField] private VideoPlayer videoPlayer;

    void Start()
    {
        GameObject[] speakerObjects = GameObject.FindGameObjectsWithTag("Speaker");

        speakers = new AudioSource[speakerObjects.Length];

        for (int i = 0; i < speakerObjects.Length; i++)
        {
            speakers[i] = speakerObjects[i].GetComponent<AudioSource>();
        }
    }

    public void ToggleMusic()
    {
        if (isplaying)
        {
            foreach (AudioSource source in speakers)
            {
                if (source != null)
                {
                    source.Pause();
                }
            }

            if (videoPlayer != null)
            {
                videoPlayer.Pause();
            }

            isplaying = false;
        }
        else
        {
            foreach (AudioSource source in speakers)
            {
                if (source != null)
                {
                    source.UnPause();
                }
            }

            if (videoPlayer != null)
            {
                videoPlayer.Play();
            }

            isplaying = true;
        }
    }
}