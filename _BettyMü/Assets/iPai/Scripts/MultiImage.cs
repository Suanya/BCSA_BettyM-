using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Video;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MultiImage : MonoBehaviour
{
    [SerializeField] private string[] arObjectPoolNames; // Names of the pools to use

    private ARTrackedImageManager m_TrackedImageManager;
    private Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>();

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

        // Initialize the dictionary with pool names
        foreach (string poolName in arObjectPoolNames)
        {
            arObjects.Add(poolName, null); // Initialize with null to be replaced by pooled objects
        }
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateARImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateARImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            HandleRemovedImage(trackedImage);
        }
    }

    private void UpdateARImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        GameObject arObject = GetOrCreateGameObject(name);

        if (arObject != null)
        {
            arObject.transform.position = trackedImage.transform.position;
            arObject.transform.rotation = trackedImage.transform.rotation;
            arObject.SetActive(true);

            PlayVideoAndAudio(arObject);
        }
    }

    private void HandleRemovedImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;

        if (arObjects.TryGetValue(name, out GameObject arObject) && arObject != null)
        {
            StopVideoAndAudio(arObject);
            PoolManager.Instance.ReturnObject(name, arObject);
            arObjects[name] = null; // Set to null so a new object can be fetched next time
        }
    }

    private GameObject GetOrCreateGameObject(string name)
    {
        if (arObjects.TryGetValue(name, out GameObject arObject) && arObject != null)
        {
            return arObject;
        }

        arObject = PoolManager.Instance.GetObject(name);
        if (arObject != null)
        {
            arObjects[name] = arObject;
            SetupVideoPlayer(arObject);
        }

        return arObject;
    }

    private void PlayVideoAndAudio(GameObject arObject)
    {
        var videoPlayer = arObject.GetComponent<VideoPlayer>();
        if (videoPlayer != null && !videoPlayer.isPlaying)
        {
            videoPlayer.Play();
            var audioSource = arObject.GetComponent<AudioSource>();
            audioSource?.Play();
        }
    }

    private void StopVideoAndAudio(GameObject arObject)
    {
        var videoPlayer = arObject.GetComponent<VideoPlayer>();
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            var audioSource = arObject.GetComponent<AudioSource>();
            audioSource?.Stop();
        }
    }

    private void SetupVideoPlayer(GameObject arObject)
    {
        var videoPlayer = arObject.GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;

            var audioSource = arObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = arObject.AddComponent<AudioSource>();
            }

            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }
    }
}
