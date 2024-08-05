using UnityEngine;
using UnityEngine.Video;

public class MuteOnOpen : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct; // Direct audio output
            videoPlayer.SetDirectAudioMute(0, true); // Ensure it starts muted
        }
    }

    void OnEnable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.SetDirectAudioMute(0, false); // Unmute when enabled
            videoPlayer.Play();
            Debug.Log("VideoPlayer started playing on Enable.");
        }
    }

    void OnDisable()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            videoPlayer.clip = null; // Release video clip
            Debug.Log("VideoPlayer stopped and clip released on Disable.");
        }
    }
}
