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
            GameObject customGridObj = parentObj.transform.parent.gameObject;
            
            float value = (float) CustomGrid.GetValue(gameObject.transform.localPosition,1.01f);
            if (customGridObj.CompareTag("customGrid"))
            {
                ScaleCube(customGridObj.GetComponent<HeatMapBehavior>(),value);
            }
        }
        else
        {
            UnScaleAll();
        }
    }

    private void UnScaleAll()
    {
        foreach (HeatMapBehavior heatMap in heatMapBehaviors)
        {
            heatMap.UnScaleChart();
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
                heatMapBehavior.UnScaleChart();
            }
        }
    }
}
