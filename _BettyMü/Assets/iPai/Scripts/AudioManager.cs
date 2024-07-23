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
        // If there is already a video playing, stop its audio track
        if (currentlyPlayingVideo != null && currentlyPlayingVideo.isPlaying)
        {
            currentlyPlayingVideo.SetDirectAudioMute(0, true);
        }

        // Play the new video and ensure its audio is unmuted
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
