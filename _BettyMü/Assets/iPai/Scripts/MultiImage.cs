using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Video;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MultiImage : MonoBehaviour
{
    [SerializeField] private string[] arObjectPoolNames;

    private ARTrackedImageManager m_TrackedImageManager;
    private Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>();

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

        foreach (string poolName in arObjectPoolNames)
        {
            arObjects.Add(poolName, null);
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

            VideoPlayer videoPlayer = arObject.GetComponent<VideoPlayer>();
            if (videoPlayer != null)
            {
                AudioManager.Instance.RegisterVideoPlayer(videoPlayer);
                AudioManager.Instance.PlayVideo(videoPlayer);
            }
            else
            {
                Debug.LogWarning("No VideoPlayer found on the GameObject.");
            }
        }
        else
        {
            Debug.LogWarning($"No AR object found for {name}.");
        }
    }

    private void HandleRemovedImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        if (arObjects.TryGetValue(name, out GameObject arObject) && arObject != null)
        {
            VideoPlayer videoPlayer = arObject.GetComponent<VideoPlayer>();
            if (videoPlayer != null)
            {
                AudioManager.Instance.UnregisterVideoPlayer(videoPlayer);
                AudioManager.Instance.StopVideo(videoPlayer);
            }
            arObject.SetActive(false);
            PoolManager.Instance.ReturnObject(name, arObject);
            arObjects[name] = null;
        }
    }

    private GameObject GetOrCreateGameObject(string name)
    {
        if (arObjects.ContainsKey(name))
        {
            if (arObjects[name] == null)
            {
                arObjects[name] = PoolManager.Instance.GetObject(name);
            }
            return arObjects[name];
        }

        Debug.LogError($"No pool exists with the name: {name}");
        return null;
    }
}
