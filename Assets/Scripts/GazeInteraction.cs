using Assets.Scripts;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using System.Collections;

public class GazeInteraction : MonoBehaviour
{
    List<GameObject> gameObjList = new List<GameObject>();
    IDictionary<GameObject, Color> visitedGrid = new Dictionary<GameObject, Color>();
    [SerializeField]
    private ARSessionOrigin aRSessionOrigin;

    [SerializeField]
    private TextMeshPro addInfoText;
    [SerializeField]
    private GameObject addInfoWindow;


    void Start()
    {
        gameObjList = GameObject.FindGameObjectsWithTag(GridArray.cubeTag).ToList();
        HideAddInfoWindow();
    }

    void Update()
    {
        if (gameObjList.Count == 0)
        {
            gameObjList = GameObject.FindGameObjectsWithTag(GridArray.cubeTag).ToList();
        }

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            GameObject gameObject = hit.collider.gameObject;
            
            if (gameObject.CompareTag("customGrid"))
            {
                HighLightCube(gameObject);
            }
        }
        else
        {
            HideAddInfoWindow();
            UnHeighLightAll();
        }
    }

    private void UnHeighLightAll()
    {
        foreach (GameObject gameObj in gameObjList)
        {
            Material material = gameObj.GetComponent<MeshRenderer>().material;
            if (visitedGrid.ContainsKey(gameObj))
            {
                material.color = visitedGrid[gameObj];
            }
        }
    }

    private void ShowAddInfoWindow(GameObject gameObj)
    {
        string text = "";
        InfoData infoList = gameObj.GetComponent<InfoData>();

        if (infoList.infoDataList.Count > 0)
        {
            foreach (string item in infoList.infoDataList)
            {
                text += item;
            }
            Vector3 pos = gameObj.transform.position;

            addInfoText.text = text;
            addInfoWindow.transform.localPosition = pos;
            addInfoWindow.gameObject.SetActive(true);
        }
    }

    private void HideAddInfoWindow()
    {
        addInfoText.text = default;
        addInfoWindow.gameObject.SetActive(false);
    }

    void HighLightCube(GameObject desiredHeatmap)
    {
        foreach (GameObject gameObj in gameObjList)
        {
            Material material = gameObj.GetComponent<MeshRenderer>().material;
            Color originalColor = material.color;
            if (gameObj == desiredHeatmap)
            {
                material.color = Color.red;
                if (!visitedGrid.ContainsKey(gameObj))
                {
                    visitedGrid.Add(gameObj, originalColor);
                }

                ShowAddInfoWindow(gameObj);
            }
            else
            {
                if (visitedGrid.ContainsKey(gameObj))
                {
                    material.color = visitedGrid[gameObj];
                }
                else
                {
                    material.color = originalColor;
                }
            }
        }
    }
}
