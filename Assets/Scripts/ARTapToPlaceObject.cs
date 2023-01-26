using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System;
using UnityEngine.XR.ARSubsystems;
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
    //private ARRaycastManager arOrigin;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    private char lineSeperater = '\n'; // It defines line seperate character

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        spawnedObject = null;
        string[] dataList = csvFile.text.Split(lineSeperater, StringSplitOptions.RemoveEmptyEntries);
        new CustomGrid(objectToPlace, gridSize, dataList);
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
         Debug.Log("Before Touch Position");

        if(Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
        {
            Debug.LogWarning("Touch Position : " + placementPose.position);

            if (Physics.Raycast(ray,out raycastHit))
            {
                if (raycastHit.collider.gameObject.tag == "heatMap")
                {
                    spawnedObject = raycastHit.collider.gameObject;
                }
                else
                {
                    //objectToPlace.SetActive(true);
                    spawnedObject = objectToPlace;
                    spawnedObject.transform.position = placementPose.position;
                    spawnedObject.SetActive(true); 
                    Debug.LogWarning("After Touch Object Position : " + spawnedObject.transform.position);

                    //spawnedObject = Instantiate(objectToPlace, m_Hits[0].pose.position, Quaternion.identity);
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
         

         /*Debug.LogWarning("Before Touch Position" + placementPose.position);
         if ((placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (placementPoseIsValid && Input.GetMouseButton(0)))
         {
             Debug.LogWarning("Touch Position" + placementPose.position);
             PlaceObject();
             objectToPlace.SetActive(true);
             Debug.LogWarning("After Touch Object Position : " + objectToPlace.transform.position);
        }
         Debug.LogWarning("After Touch Position" + placementPose.position);*/

    }

    private void PlaceObject()
    {
        Debug.LogWarning("Position" + placementPose.position);
        Debug.LogWarning("Rotation" + placementPose.rotation);
        objectToPlace.transform.position = placementPose.position;
        Debug.LogWarning("objectToPlace Position" + objectToPlace.transform.position);
        Debug.LogWarning("objectToPlace Rotation" + objectToPlace.transform.rotation);
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
        // Vector3 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        // arOrigin = FindObjectOfType<ARRaycastManager>();
        //Ray ray = camera.ScreenPointToRay(Input.GetTouch(0).position);
        //arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);
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
