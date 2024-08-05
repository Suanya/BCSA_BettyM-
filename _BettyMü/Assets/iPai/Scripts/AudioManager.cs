using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private VideoPlayer currentlyPlayingVideo;
    private List<VideoPlayer> allVideoPlayers = new List<VideoPlayer>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes if needed
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterVideoPlayer(VideoPlayer videoPlayer)
    {
        if (!allVideoPlayers.Contains(videoPlayer))
        {
            allVideoPlayers.Add(videoPlayer);
        }
    }

    public void UnregisterVideoPlayer(VideoPlayer videoPlayer)
    {
        if (allVideoPlayers.Contains(videoPlayer))
        {
            allVideoPlayers.Remove(videoPlayer);
        }
    }

    public void PlayVideo(VideoPlayer newVideo)
    {
        if (currentlyPlayingVideo != null)
        {
            // Mute the currently playing video
            currentlyPlayingVideo.SetDirectAudioMute(0, true);
        }

        // Set and play the new video
        currentlyPlayingVideo = newVideo;
        if (currentlyPlayingVideo != null)
        {
            currentlyPlayingVideo.audioOutputMode = VideoAudioOutputMode.Direct; // Direct audio output
            currentlyPlayingVideo.SetDirectAudioMute(0, false); // Unmute the current video
            currentlyPlayingVideo.Play();
            Debug.Log("Started playing new video.");

            // Mute all other video players
            foreach (var videoPlayer in allVideoPlayers)
            {
                if (videoPlayer != currentlyPlayingVideo)
                {
                    videoPlayer.SetDirectAudioMute(0, true);
                }
            }
        }
    }

    public void StopVideo(VideoPlayer videoToStop)
    {
        if (videoToStop == currentlyPlayingVideo)
        {
            currentlyPlayingVideo.SetDirectAudioMute(0, true); // Mute audio
            currentlyPlayingVideo.Stop();
            currentlyPlayingVideo.clip = null; // Release video clip
            currentlyPlayingVideo = null;
            Debug.Log("Stopped video and released clip.");

            // Optionally, unmute the next available video if any
            foreach (var videoPlayer in allVideoPlayers)
            {
                if (videoPlayer != null && videoPlayer.isPlaying)
                {
                    currentlyPlayingVideo = videoPlayer;
                    videoPlayer.SetDirectAudioMute(0, false);
                    break;
                }
            }
        }
    }
}
