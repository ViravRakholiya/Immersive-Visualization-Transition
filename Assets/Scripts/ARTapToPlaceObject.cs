using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class ARTapToPlaceObject : MonoBehaviour
{
    public TextAsset csvFile;
    public GameObject gameObjectToInstantiate;

    private char lineSeperater = '\n'; // It defines line seperate character

    private GameObject parentObject;
    private GameObject spawnObj;
    private ARRaycastManager _aRRaycastManager;
    private Vector2 touchPosition;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    string[] dataList;


    void Start()
    {
        dataList = csvFile.text.Split(lineSeperater, StringSplitOptions.RemoveEmptyEntries);
        parentObject = new GameObject("HeatMap");
        new CustomGrid(parentObject, 5f, dataList);
    }

    private void Awake()
    {
        _aRRaycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    private void Update()
    {
       if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (_aRRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            if (spawnObj == null)
            { 
                spawnObj =  Instantiate(parentObject, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnObj.transform.position = hitPose.position;
            }
        }
    }
}
