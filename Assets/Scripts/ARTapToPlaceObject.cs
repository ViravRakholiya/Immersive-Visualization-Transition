using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System;
using Assets.Scripts;

[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlaceObject : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager arOrigin;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public TextAsset csvFile;
    public GameObject placementIndicator;
    public float gridSize = 1f;

    Camera cam;



    public GameObject objectToPlace;
    GameObject spawnedObject;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    private char lineSeperater = '\n'; // It defines line seperate character

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        spawnedObject = null;
        string[] dataList = csvFile.text.Split(lineSeperater, StringSplitOptions.RemoveEmptyEntries);
        new CustomGrid(objectToPlace, gridSize, dataList,0);
        objectToPlace.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.touchCount == 0)
         {
             return;
         }

        UpdatePlacementPose();
        UpdatePlacementIndicator();

         RaycastHit raycastHit;
         Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);

        if(Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
        {
            if (Physics.Raycast(ray,out raycastHit))
            {
                if (raycastHit.collider.gameObject.tag == "heatMap")
                {
                    spawnedObject = raycastHit.collider.gameObject;
                }
                else
                {
                    spawnedObject = objectToPlace;
                    spawnedObject.transform.position = placementPose.position;
                    spawnedObject.SetActive(true);
                }
            }
        }
        else if(Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObject != null)
        {
            spawnedObject.transform.position = placementPose.position;
        }

        if (Input.GetTouch(0).phase == TouchPhase.Ended )
        {
            spawnedObject = null;
        }

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
        Debug.Log("1 Position" + placementPose.position);
        arOrigin.Raycast(Input.GetTouch(0).position, hits);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = cam.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
