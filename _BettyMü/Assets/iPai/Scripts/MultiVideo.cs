using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]

public class MultiVideo : MonoBehaviour
{
    [SerializeField] private GameObject[] arObjectsToPlace;

    private ARTrackedImageManager m_TrackedImageManager;
    private Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, VideoPlayer> arVideoPlayers = new Dictionary<string, VideoPlayer>();

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

        // setup all game objects in dictionary
        foreach (GameObject arObject in arObjectsToPlace)
        {
            GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
            newARObject.name = arObject.name;
            arObjects.Add(arObject.name, newARObject);

            // Assuming each AR object has a VideoPlayer component
            VideoPlayer videoPlayer = newARObject.GetComponent<VideoPlayer>();
            if (videoPlayer != null)
            {
                arVideoPlayers.Add(arObject.name, videoPlayer);
                videoPlayer.playOnAwake = false;
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
            arObjects[trackedImage.referenceImage.name].SetActive(false);
            StopVideo(trackedImage.referenceImage.name);
        }
    }

    private void UpdateARImage(ARTrackedImage trackedImage)
    {
        AssignGameObject(trackedImage.referenceImage.name, trackedImage.transform.position, trackedImage.transform.rotation);
        Debug.Log($"trackedImage.referenceImage.name: {trackedImage.referenceImage.name}");
    }

    void AssignGameObject(string name, Vector3 newPosition, Quaternion newRotation)
    {
        if (arObjectsToPlace != null)
        {
            GameObject goARObject = arObjects[name];
            goARObject.SetActive(true);
            goARObject.transform.position = newPosition;

            // Apply new rotation
            Quaternion offsetRotation = Quaternion.Euler(0, 16, 0);
            goARObject.transform.rotation = newRotation * offsetRotation;

            // Start the video for the current object
            PlayVideo(name);
        }
    }

    private async void PlayVideo(string name)
    {
        if (arVideoPlayers.ContainsKey(name))
        {
            var videoPlayer = arVideoPlayers[name];
            if (!videoPlayer.isPlaying)
            {
                videoPlayer.Prepare();
                while (!videoPlayer.isPrepared)
                {
                    await Task.Yield(); // Wait until video is prepared
                }
                videoPlayer.Play();
            }
        }
    }

    private void StopVideo(string name)
    {
        if (arVideoPlayers.ContainsKey(name))
        {
            var videoPlayer = arVideoPlayers[name];
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
        }
    }
}



