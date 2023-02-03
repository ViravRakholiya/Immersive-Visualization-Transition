using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class AnchorCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject m_AnchorPrefab;

    private char lineSeperater = '\n';

    public GameObject AnchorPrefab
    {
        get => m_AnchorPrefab;
        set => m_AnchorPrefab = value;
    }

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
        StreamReader reader = new StreamReader(MapInfo.filePath);
        string[] dataList = reader.ReadToEnd().Split(lineSeperater, StringSplitOptions.RemoveEmptyEntries);
        new CustomGrid(m_AnchorPrefab, MapInfo.gridSize, dataList, MapInfo.noOfColoumnforAddInfo);
        m_AnchorPrefab.SetActive(false);
    }

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

        if (Input.touchCount == 0)
            return;

        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began)
            return;

           if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
           {
                var hitPose = s_Hits[0].pose;
                var hitTrackableId = s_Hits[0].trackableId;
                var hitPlane = m_PlaneManager.GetPlane(hitTrackableId);
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
