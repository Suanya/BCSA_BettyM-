using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]

public class MultiVideo : MonoBehaviour
{
    [SerializeField]
    private GameObject[] arObjectsToPlace;
    [SerializeField]
    private GameObject arVideoPrefab; // Reference to the AR video prefab

    private ARTrackedImageManager m_TrackedImageManager;
    private Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>();
    private GameObject currentARVideo; // Reference to the currently active AR video GameObject

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

        // Setup all game objects in dictionary
        foreach (GameObject arObject in arObjectsToPlace)
        {
            GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
            newARObject.name = arObject.name;
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
            HideARVideo();
            arObjects[trackedImage.referenceImage.name].SetActive(false);
        }
    }

    private void UpdateARImage(ARTrackedImage trackedImage)
    {
        // Assign and Place Game Object
        AssignGameObject(trackedImage.referenceImage.name, trackedImage.transform.position);

        // If the tracked image is the round picture, show AR video
        if (trackedImage.referenceImage.name == "RoundPicture")
        {
            ShowARVideo(trackedImage.transform.position, trackedImage.transform.rotation);
        }
    }

    void AssignGameObject(string name, Vector3 newPosition)
    {
        if (arObjectsToPlace != null)
        {
            GameObject goARObject = arObjects[name];
            goARObject.SetActive(true);
            goARObject.transform.position = newPosition;
            foreach (GameObject go in arObjects.Values)
            {
                if (go.name != name)
                {
                    go.SetActive(false);
                }
            }
        }
    }

    void ShowARVideo(Vector3 position, Quaternion rotation)
    {
        // If an AR video is already active, destroy it
        if (currentARVideo != null)
        {
            Destroy(currentARVideo);
        }


        // Calculate additional rotation adjustment to align the video with the tracked image
        //Quaternion additionalRotation = Quaternion.Euler(90f, 0f, 0f); // Example adjustment, adjust as needed

        // Apply the additional rotation adjustment to the provided rotation
        //Quaternion finalRotation = rotation * additionalRotation;

        // Instantiate and place the AR video prefab at the round picture's position and rotation
        currentARVideo = Instantiate(arVideoPrefab, position, rotation);
    }

    void HideARVideo()
    {
        // If an AR video is active, destroy it
        if (currentARVideo != null)
        {
            Destroy(currentARVideo);
            currentARVideo = null;
        }
    }
}