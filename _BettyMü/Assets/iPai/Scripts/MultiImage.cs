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

        // setup all game objects in dictionary
        foreach (GameObject arObject in arObjectsToPlace)
        {
            GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
            newARObject.name = arObject.name;
            newARObject.SetActive(false);
            arObjects.Add(arObject.name, newARObject);
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
            if (arObjects.TryGetValue(trackedImage.referenceImage.name, out GameObject arObject))
            {
                var videoPlayer = arObject.GetComponent<VideoPlayer>();
                if (videoPlayer != null)
                {
                    videoPlayer.Stop();
                }
                arObject.SetActive(false);
            }
        }
    }

    private void UpdateARImage(ARTrackedImage trackedImage)
    {
        // Assign and Place Game Object
        AssignGameObject(trackedImage.referenceImage.name, trackedImage.transform.position, trackedImage.transform.rotation);
        Debug.Log($"trackedImage.referenceImage.name: {trackedImage.referenceImage.name}");
    }

    void AssignGameObject(string name, Vector3 newPosition, Quaternion newRotation)
    {
        if (arObjects.TryGetValue(name, out GameObject goARObject))
        {
            goARObject.SetActive(true);
            goARObject.transform.position = newPosition;
            goARObject.transform.rotation = newRotation;

            // Ensure only one video plays at a time
            foreach (var arObject in arObjects.Values)
            {
                var videoPlayer = arObject.GetComponent<VideoPlayer>();
                if (videoPlayer != null)
                {
                    if (arObject.name == name)
                    {
                        if (!videoPlayer.isPlaying)
                        {
                            videoPlayer.Play();
                        }
                    }
                    else
                    {
                        if (videoPlayer.isPlaying)
                        {
                            videoPlayer.Stop();
                        }
                        arObject.SetActive(false);
                    }
                }
            }
        }
    }
}
