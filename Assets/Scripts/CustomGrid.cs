using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class CustomGrid
    {
        private int width;
        private int height;
        string[] dataList;
        private List<double> valueList;

        private char fieldSeperator = ',';

        private Material material;
        public CustomGrid(GameObject parentObject,float m_cellSize, string[] dataList)
        {
            GridArray.cellSize = m_cellSize;
            this.dataList = dataList;

            width = dataList[0].Split(fieldSeperator).Length; // Number of coloumn in csv file
            height = dataList.Length; // Number of row in csv files

            GridArray.gridArrayList = new double[height, width];
            material = Resources.Load<Material>("CubeColor");
            CreateHeatMap(parentObject);
        }

        /// <summary>
        /// Get list of only numeric value from datalist
        /// </summary>
        public void GetValueList()
        {
            valueList = new List<double>();

            for (int x = 1; x < height; x++)
            {
                string[] data = dataList[x].Split(fieldSeperator);

                for (int y = 1; y < width; y++)
                {
                    if (x != 0 && y != 0)
                    {
                        double value = 0f;
                        string fData = data[y];

                        if (!double.TryParse(fData, out value))
                        {
                            value = 0f;
                        }

                        valueList.Add(value);
                    }
                }
            }
        }

        public void CreateHeatMap(GameObject parentObject)
        {
            GetValueList();

            ColorHeatMap colorHeatMap = new ColorHeatMap(valueList.Min(), valueList.Max(),8);

            for (int x = 0; x < height; x++)
            {
                string[] data = dataList[x].Split(fieldSeperator);

                for(int y = 0; y < width; y++)
                {
                    // Removing Header
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    GameObject cubeParentObject = new GameObject("Parent" + x + y);
                    cubeParentObject.transform.SetParent(parentObject.transform, false);

                    GameObject cubeObject = new GameObject("Child" + x + y);
                    cubeObject.transform.SetParent(cubeParentObject.transform, false);

                    Vector3 textPos = new Vector3(0f, 0f, 0.5f);

                    if (x == 0 && y != 0)
                    {
                       CreateCubeWithText(data[y], new Color(1f, 1f, 1f, 0f), Color.white, cubeObject.transform, GetWorldPosition(x, y), textPos, new Vector3(GridArray.cellSize, GridArray.cellSize, 0.01f), new Vector3(0.2f, 0.2f, 1f), Quaternion.Euler(0f, 0f, 0f), 20, TextAnchor.MiddleRight);
                    }
                    else if (x != 0 && y != 0)
                    {
                        double value = 0f;
                        string fData = data[y];

                        if (!double.TryParse(fData, out value))
                        {
                            value = 0f;
                        }

                        cubeParentObject.tag = "customGrid";
                        cubeParentObject.AddComponent(typeof(HeatMapBehavior));
                        cubeParentObject.AddComponent(typeof(BoxCollider));

                        Vector3 worldPosition = GetWorldPosition(x, y, 1.01f);
                        SetValue(worldPosition, value); ;
                        Color cubeColor = colorHeatMap.GetColorForValue(value, valueList.Min(), valueList.Max());

                        CreateCubeWithText(value.ToString("0.00"), cubeColor, Color.black, cubeObject.transform, worldPosition, textPos, new Vector3(GridArray.cellSize, GridArray.cellSize, 0.01f), new Vector3(0.2f, 0.2f, 1f), Quaternion.Euler(0f, 0f, 00f), 20, TextAnchor.MiddleCenter, TextAlignment.Left);

                        HeatMapBehavior heatMapBehaviorObj = cubeParentObject.GetComponent<HeatMapBehavior>();
                        heatMapBehaviorObj.particularCubeTransform = cubeObject.transform;
                    }
                    else
                    {
                       CreateCubeWithText(data[y], new Color(1f,1f,1f,0f), Color.white, cubeObject.transform, GetWorldPosition(x, y), textPos, new Vector3(GridArray.cellSize, GridArray.cellSize, 0.01f), new Vector3(0.2f, 0.2f, 1f), Quaternion.Euler(0f, 0f, 90f),20,TextAnchor.MiddleRight);
                    }
                }
            }
        }


        /// <summary>
        /// Method to set value of cell
        /// </summary>
        /// <param name="x">x cordinates of the position</param>
        /// <param name="y">y cordinates of the position</param>
        /// <param name="value">value from the dataset</param>
        public void SetValue(int x, int y, double value)
        {
            GridArray.gridArrayList[x, y] = value;
        }

        public void SetValue(Vector3 worldPosition, double value, float gridspace = 1)
        {
            int x, y;
            GetXY(worldPosition, out x, out y,gridspace);
            SetValue(x, y, value);
        }

        /// <summary>
        /// Method to get value of cell
        /// </summary>
        /// <param name="x">x cordinates of the position</param>
        /// <param name="y">y cordinates of the position</param>
        /// <returns>Return the value of current position</returns>
        public static double GetValue(int x, int y)
        {
            return GridArray.gridArrayList[x, y];
        }

        public static double GetValue(Vector3 worldPosition, float gridspace = 1)
        {
            int x, y;
            GetXY(worldPosition, out x, out y, gridspace);
            return GetValue(x, y);
        }

        private static void GetXY(Vector3 worldPosition, out int x, out int y, float gridspace = 1)
        {
            x = Mathf.FloorToInt((worldPosition).x  / (GridArray.cellSize * gridspace));
            y = Mathf.FloorToInt((worldPosition).y / (GridArray.cellSize * gridspace));
        }

        private Vector3 GetWorldPosition(int x, int y, float gridspace = 1)
        {
            return new Vector3(x * gridspace, y * gridspace) * GridArray.cellSize;
        }

        // Create Cube with text in the World
        private GameObject CreateCubeWithText(string text, Color Cubecolor,Color fontColor, Transform parent = null, Vector3 cubePosition = default(Vector3), Vector3 textPosition = default(Vector3),Vector3 CubeScale = default(Vector3), Vector3 textScale = default(Vector3), Quaternion textRotation = default(Quaternion), int fontSize = 20, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.name = "Cube"+text;
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            cubePosition.z = CubeScale.z / 2;
            transform.localPosition = cubePosition;
            transform.localScale = CubeScale;
            MeshRenderer rend = gameObject.GetComponent<MeshRenderer>();
            rend.material = material;
            rend.receiveShadows = false;
            rend.material.color = Cubecolor;

            if (Cubecolor.a == 0f)
            {
               /* rend.material.SetOverrideTag("RenderType", "Transparent");
                rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                rend.material.SetInt("_ZWrite", 0);
                rend.material.DisableKeyword("_ALPHATEST_ON");
                rend.material.EnableKeyword("_ALPHABLEND_ON");
                rend.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                rend.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;*/
            }


            // rend.material.color = Cubecolor;
            //  rend.material.SetColor("_Color", Cubecolor);


            if (Cubecolor.a == 0f)
            {
                gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
            
            CreateWorldText(text, fontColor, gameObject.transform, textPosition, textRotation, textScale, fontSize, textAnchor, textAlignment);

            return gameObject;
        }

        // Create Text in the World
        private TextMesh CreateWorldText(string text, Color fontColor, Transform parent = null, Vector3 localPosition = default(Vector3), Quaternion localRotation = default(Quaternion), Vector3 textScale = default(Vector3),int fontSize = 20, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
        {
            GameObject gameObject = new GameObject("Value" +text, typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            transform.localScale = textScale;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = fontColor;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            return textMesh;
        }
    }
}