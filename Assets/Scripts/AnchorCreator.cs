using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

//
// This script allows us to create anchors with
// a prefab attached in order to visbly discern where the anchors are created.
// Anchors are a particular point in space that you are asking your device to track.
//

[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class AnchorCreator : MonoBehaviour
{
    // This is the prefab that will appear every time an anchor is created.
    [SerializeField]
    GameObject m_AnchorPrefab;
    public TextAsset csvFile;
    public float gridSize = 1f;
    public int noOfColoumnOfAddInfo = 0;

    private char lineSeperater = '\n';

    public GameObject AnchorPrefab
    {
        get => m_AnchorPrefab;
        set => m_AnchorPrefab = value;
    }

    // Removes all the anchors that have been created.
    public void RemoveAllAnchors()
    {
        foreach (var anchor in m_AnchorPoints)
        {
            Destroy(anchor);
        }
        m_AnchorPoints.Clear();
    }

    void Start()
    {
        string[] dataList = csvFile.text.Split(lineSeperater, StringSplitOptions.RemoveEmptyEntries);
        new CustomGrid(m_AnchorPrefab, gridSize, dataList,noOfColoumnOfAddInfo);
        m_AnchorPrefab.SetActive(false);
    }

    // On Awake(), we obtains a reference to all the required components.
    // The ARRaycastManager allows us to perform raycasts so that we know where to place an anchor.
    // The ARPlaneManager detects surfaces we can place our objects on.
    // The ARAnchorManager handles the processing of all anchors and updates their position and rotation.
    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_PlaneManager = GetComponent<ARPlaneManager>();
        m_AnchorPoints = new List<ARAnchor>();
    }

    void Update()
    {

        if (m_AnchorPrefab.activeSelf)
        {
            DisablePlaneDetection();
        }

        // If there is no tap, then simply do nothing until the next call to Update().
       if (Input.touchCount == 0)
            return;

       var touch = Input.GetTouch(0);
       if (touch.phase != TouchPhase.Began)
            return;

       if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
       {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;
            var hitTrackableId = s_Hits[0].trackableId;
            var hitPlane = m_PlaneManager.GetPlane(hitTrackableId);

            // This attaches an anchor to the area on the plane corresponding to the raycast hit,
            // and afterwards instantiates an instance of your chosen prefab at that point.
            // This prefab instance is parented to the anchor to make sure the position of the prefab is consistent
            // with the anchor, since an anchor attached to an ARPlane will be updated automatically by the ARAnchorManager as the ARPlane's exact position is refined.
            var anchor = m_AnchorManager.AttachAnchor(hitPlane, hitPose);
            m_AnchorPrefab.transform.position = anchor.transform.position;
            m_AnchorPrefab.SetActive(true);
            if (hitPlane.alignment == PlaneAlignment.HorizontalDown || hitPlane.alignment == PlaneAlignment.HorizontalUp)
            {
                 m_AnchorPrefab.transform.Rotate(new Vector3(90, 0, 0));
            }

            StartCoroutine("ScallAll");

          if (anchor == null)
          {
              Debug.Log("Error creating anchor.");
          }
          else
          {
              // Stores the anchor so that it may be removed later.
              m_AnchorPoints.Add(anchor);
          }
       }
    }

    private void DisablePlaneDetection()
    {
        foreach(ARPlane plane in m_PlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }

    IEnumerator ScallAll()
    {
        for (int i = 0; i < m_AnchorPrefab.transform.childCount; i++)
        {
            Transform gameObjTrans = m_AnchorPrefab.transform.GetChild(i);
            float value = (float)CustomGrid.GetValue(gameObjTrans.GetChild(0).GetChild(0).localPosition);
            if (value > 0.01)
            {
                Vector3 valuePosition = gameObjTrans.GetChild(0).GetChild(0).GetChild(0).localPosition;
                valuePosition.z = 0.515f;
                gameObjTrans.GetChild(0).GetChild(0).GetChild(0).localPosition = valuePosition;
            }
            if (value != 0)
            {
                Vector3 desiredScale = gameObjTrans.GetChild(0).localScale;
                desiredScale.z = (-value);
                gameObjTrans.GetChild(0).localScale = desiredScale;
            }
            yield return null;
        }
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_AnchorPoints;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;

    ARPlaneManager m_PlaneManager;
}
