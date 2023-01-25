using Assets.Scripts;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GazeInteraction : MonoBehaviour
{
    List<HeatMapBehavior> heatMapBehaviors = new List<HeatMapBehavior>();

    void Start()
    {
        heatMapBehaviors = FindObjectsOfType<HeatMapBehavior>().ToList();
    }

    void Update()
    {
        if(heatMapBehaviors.Count == 0)
        {
            heatMapBehaviors = FindObjectsOfType<HeatMapBehavior>().ToList();
        }

        if (Physics.Raycast(transform.position,transform.forward,out RaycastHit hit))
        {
            GameObject gameObject = hit.collider.gameObject;
            GameObject parentObj = gameObject.transform.parent.gameObject;
            GameObject childObj = parentObj.transform.parent.gameObject;
            Debug.Log("Parent Object Name: "+parentObj.name);
            float value = float.Parse(gameObject.name.Remove(0, 4));

            if (childObj.CompareTag("customGrid"))
            {
                ScaleCube(childObj.GetComponent<HeatMapBehavior>(),value);
            }
        }
    }

    void ScaleCube(HeatMapBehavior desiredHeatmap,float desiredScale)
    {
        foreach (HeatMapBehavior heatMapBehavior in heatMapBehaviors)
        {
            if (heatMapBehavior == desiredHeatmap)
            {
                heatMapBehavior.ScaleChart(desiredScale);
            }
            else
            {
                heatMapBehavior.UnScaleChart(desiredScale);
            }
        }
    }
}
