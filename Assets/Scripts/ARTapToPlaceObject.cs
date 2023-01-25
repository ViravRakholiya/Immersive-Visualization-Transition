using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System;
using UnityEngine.XR.ARSubsystems;
using Assets.Scripts;

public class ARTapToPlaceObject : MonoBehaviour
{
    public TextAsset csvFile;
    public GameObject placementIndicator;
    public float gridSize = 1f;


    public GameObject objectToPlace;
    private ARRaycastManager arOrigin;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    private char lineSeperater = '\n'; // It defines line seperate character

    // Start is called before the first frame update
    void Start()
    {
       string[] dataList = csvFile.text.Split(lineSeperater, StringSplitOptions.RemoveEmptyEntries);
       new CustomGrid(objectToPlace, gridSize, dataList);
       objectToPlace.SetActive(false);
        arOrigin = FindObjectOfType<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if ((placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (placementPoseIsValid && Input.GetMouseButton(0)))
        {
            PlaceObject();
            objectToPlace.SetActive(true);
        }
    }

    private void PlaceObject()
    {
        Debug.Log("Position" + placementPose.position);
        Debug.Log("Rotation" + placementPose.rotation);
        objectToPlace.transform.position = placementPose.position;
        Debug.Log("objectToPlace Position" + objectToPlace.transform.position);
        Debug.Log("objectToPlace Rotation" + objectToPlace.transform.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
            placementIndicator.SetActive(true);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            // use the input stuff
            if (Input.GetMouseButton(0))
            {
                placementPose.position = Input.mousePosition;
                var cameraForward = Camera.current.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
                placementPoseIsValid = true;
            }
        }
        else
        {
            Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);

            placementPoseIsValid = hits.Count > 0;
            if (placementPoseIsValid)
            {
                placementPose = hits[0].pose;

                var cameraForward = Camera.current.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }
        }
        Debug.Log("Position" + placementPose.position);
        Debug.Log("Rotation" + placementPose.rotation);
    }
}
