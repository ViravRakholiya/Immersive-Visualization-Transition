using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class HeatMapBehavior : MonoBehaviour
    {
        const float SPEED = 6f;

        [SerializeField]
        public Transform particularCubeTransform;

        Vector3 desiredScale = Vector3.one;

        void Update()
        {
            particularCubeTransform.localScale = Vector3.Lerp(particularCubeTransform.localScale, desiredScale, Time.deltaTime * SPEED);
        }

        public void ScaleChart(float scaleVal)
        {
            desiredScale = new Vector3(particularCubeTransform.localScale.x, particularCubeTransform.localScale.y,-scaleVal);
        }

        public void UnScaleChart(float scaleVal)
        {
            desiredScale = Vector3.one;
        }
    }
}