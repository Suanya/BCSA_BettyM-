using UnityEngine;
using UnityEngine.Video;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private VideoPlayer currentlyPlayingVideo;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayVideo(VideoPlayer newVideo)
    {
        if (currentlyPlayingVideo != null && currentlyPlayingVideo.isPlaying)
        {
            currentlyPlayingVideo.SetDirectAudioMute(0, true);
        }

        currentlyPlayingVideo = newVideo;
        currentlyPlayingVideo.SetDirectAudioMute(0, false);
        currentlyPlayingVideo.Play();
    }

    public void StopVideo(VideoPlayer videoToStop)
    {
        if (currentlyPlayingVideo == videoToStop && videoToStop.isPlaying)
        {
            videoToStop.Stop();
            currentlyPlayingVideo = null;
        }
    }
}
