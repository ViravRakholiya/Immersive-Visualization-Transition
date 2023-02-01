using Assets.Scripts;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GazeInteraction : MonoBehaviour
{
    List<GameObject> gameObjList = new List<GameObject>();
    IDictionary<GameObject, Color> visitedGrid = new Dictionary<GameObject, Color>();
    [SerializeField]
    private ARSessionOrigin aRSessionOrigin;

    private LineRenderer lineRenderer;

    void Start()
    {
        gameObjList = GameObject.FindGameObjectsWithTag(GridArray.cubeTag).ToList();
        lineRenderer = aRSessionOrigin.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (gameObjList.Count == 0)
        {
            gameObjList = GameObject.FindGameObjectsWithTag(GridArray.cubeTag).ToList();
        }

        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray,out RaycastHit hit,20f))
        {
            Debug.DrawRay(ray.origin, hit.point);
            GameObject gameObject = hit.collider.gameObject;
            
            if (gameObject.CompareTag("customGrid"))
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, ray.origin);
                lineRenderer.SetPosition(1, hit.point);
                HighLightCube(gameObject);
            }
        }
        else
        {
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
