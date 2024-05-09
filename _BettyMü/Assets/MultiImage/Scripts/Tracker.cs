using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Tracker : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager _manager;
    [SerializeField] private XRReferenceImageLibrary _library;
    [SerializeField] private List<GameObject> _trackedPrefabs;

    Dictionary<XRReferenceImage, GameObject> _trackedObjects = new Dictionary<XRReferenceImage, GameObject>();

    private void Awake()
    {
        _manager = FindObjectOfType<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        _manager.trackedImagesChanged += _manager_trackedImagesChanged;
    }

    private void OnDisable()
    {
        _manager.trackedImagesChanged -= _manager_trackedImagesChanged;
    }

    private void _manager_trackedImagesChanged(ARTrackedImagesChangedEventArgs arg)
    {
        foreach (var image in arg.added)
        {
            var idx = _library.indexOf(image.referenceImage);
            var go = Instantiate(_trackedPrefabs[idx]);
            Debug.Log($"ARTest image add #{idx} add object {go.name}");
            _trackedObjects.Add(image.referenceImage, go);
        }

        foreach (var image in arg.updated)
        {
            _trackedObjects[image.referenceImage].transform.position = image.transform.position;
            _trackedObjects[image.referenceImage].transform.rotation = image.transform.rotation;
        }

        foreach (var image in arg.removed)
        {
            Destroy(_trackedObjects[image.referenceImage]);

            Debug.Log($"ARTEST image remove #{_trackedObjects[image.referenceImage].name}");
        }


    }

}
