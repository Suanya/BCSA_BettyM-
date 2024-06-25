using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MuteOnOpen : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component not found on the GameObject.");
        }
        else
        {
            audioSource.Stop(); // Ensure the AudioSource is stopped immediately
        }

        if (videoPlayer == null)
        {
            Debug.LogWarning("VideoPlayer component not found on the GameObject.");
        }
        else
        {
            videoPlayer.playOnAwake = false; // Ensure the VideoPlayer does not play on awake
            videoPlayer.SetDirectAudioMute(0, true); // Mute the direct audio track
        }
    }

    // OnEnable is called when the object becomes enabled and active
    void OnEnable()
    {
        if (audioSource != null)
        {
            audioSource.Play(); // Play audio when the GameObject is enabled
        }

        if (videoPlayer != null)
        {
            videoPlayer.SetDirectAudioMute(0, false); // Unmute the direct audio track
            videoPlayer.Play(); // Play the video when the GameObject is enabled
        }
    }
}
