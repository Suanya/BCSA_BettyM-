using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Video;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MultiImage : MonoBehaviour
{
    [SerializeField] private GameObject[] arObjectsToPlace;

    private ARTrackedImageManager m_TrackedImageManager;
    private Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>();

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

        // Setup all game objects in the dictionary
        foreach (GameObject arObject in arObjectsToPlace)
        {
            if (arObject == null) continue;

            if (!arObjects.ContainsKey(arObject.name))
            {
                GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
                newARObject.name = arObject.name;
                newARObject.SetActive(false);
                arObjects.Add(arObject.name, newARObject);

                SetupVideoPlayer(newARObject);
            }
            else
            {
                Debug.LogWarning($"Duplicate AR object name found: {arObject.name}");
            }
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
        AssignGameObject(trackedImage.referenceImage.name, trackedImage.transform.position, trackedImage.transform.rotation);
    }

    private void HandleRemovedImage(ARTrackedImage trackedImage)
    {
        if (arObjects.TryGetValue(trackedImage.referenceImage.name, out GameObject arObject))
        {
            var videoPlayer = arObject.GetComponent<VideoPlayer>();
            var audioSource = arObject.GetComponent<AudioSource>();

            if (videoPlayer != null)
            {
                videoPlayer.Stop();
            }

            if (audioSource != null)
            {
                audioSource.Stop();
            }

            arObject.SetActive(false);
        }
    }

    private void AssignGameObject(string name, Vector3 newPosition, Quaternion newRotation)
    {
        foreach (var kvp in arObjects)
        {
            var arObject = kvp.Value;
            bool isActive = kvp.Key == name;

            arObject.SetActive(isActive);

            if (isActive)
            {
                arObject.transform.position = newPosition;
                arObject.transform.rotation = newRotation;
                PlayVideoAndAudio(arObject);
            }
            else
            {
                StopVideoAndAudio(arObject);
            }
        }
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
