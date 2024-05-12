using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

[RequireComponent(typeof(ARTrackedImageManager))]


public class MultiTracker : MonoBehaviour
{
    [SerializeField] private GameObject[] arObjectsToPlace;

    [SerializeField] private ARTrackedImageManager m_TrackedImageManager;

    [SerializeField] Vector3 scaleFactor = new Vector3(1.0f, 1.0f, 1.0f);

    private Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>();

    private void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

        // setup all objects in dictionary
        foreach (GameObject arObject in arObjectsToPlace)
        {
            GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
            newARObject.name = arObject.name;
            arObjects.Add(arObject.name, newARObject);
        }
    }

    private void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
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
            arObjects[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateARImage(ARTrackedImage trackedImage)
    {
        // Assign and Place Game
        AssignGameObject(trackedImage.referenceImage.name, trackedImage.transform.position);

        Debug.Log($"trackedImage.referenceImage.name: {trackedImage.referenceImage.name}");
    }

    private void AssignGameObject(string name, Vector3 newPosition)
    {
        if(arObjectsToPlace != null)
        {
            GameObject goARObject = arObjects[name]; 
            arObjects[name].SetActive(true);
            arObjects[name].transform.position = newPosition;
            arObjects[name].transform.localScale = scaleFactor;

            foreach(GameObject go in arObjects.Values)
            {
                Debug.Log($"Go in arObjects.Values: {go.name}");
                if(go.name != name)
                {
                    go.SetActive(false);
                }
            }
        }
    }
}
