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
        private float cellSize;
        string[] dataList;
        private double[,] gridArray;
        private List<double> valueList;

        private char fieldSeperator = ',';

        public CustomGrid(GameObject parentObject,float cellSize, string[] dataList)
        {
            this.cellSize = cellSize;
            this.dataList = dataList;

            width = dataList[0].Split(fieldSeperator).Length; // Number of coloumn in csv file
            height = dataList.Length; // Number of row in csv files

            gridArray = new double[height, width];

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

                    GameObject cubeObject = new GameObject("Cube" + x + y);
                    cubeObject.transform.SetParent(parentObject.transform, false);

                    
                    if(x == 0 && y != 0)
                    {
                        Quaternion localRotation = Quaternion.Euler(0f, 0f, 0f);
                        CreateWorldText(data[y], cubeObject.transform, GetWorldPosition(x, y), localRotation, (int)((cellSize / 2) * 10), Color.white, TextAnchor.MiddleRight);
                    }
                    else if (x != 0 && y != 0)
                    {
                        double value = 0f;
                        string fData = data[y];

                        if (!double.TryParse(fData, out value))
                        {
                            value = 0f;
                        }

                        Vector3 worldPosition = GetWorldPosition(x, y, 1.01f);
                        SetValue(worldPosition, value);
                        Color color = colorHeatMap.GetColorForValue(value, valueList.Min(), valueList.Max());
                        CreateCube(value.ToString("0.00"), color, cubeObject.transform, GetWorldPosition(x, y, 1.01f), new Vector3(cellSize, cellSize, 0.01f), (int)((cellSize /4)*10));
                    }
                    else
                    {
                        Quaternion localRotation = Quaternion.Euler(0f, 0f, 90f);
                        CreateWorldText(data[y], cubeObject.transform, GetWorldPosition(x, y), localRotation, (int)((cellSize / 2) * 10), Color.white, TextAnchor.MiddleRight);
                    }
                }
            }
        }

        public float GetCellSize()
        {
            return cellSize;
        }

        /// <summary>
        /// Method to set value of cell
        /// </summary>
        /// <param name="x">x cordinates of the position</param>
        /// <param name="y">y cordinates of the position</param>
        /// <param name="value">value from the dataset</param>
        public void SetValue(int x, int y, double value)
        {
            gridArray[x, y] = value;
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
        public double GetValue(int x, int y)
        {
            return gridArray[x, y];
        }

        public double GetValue(Vector3 worldPosition, float gridspace = 1)
        {
            int x, y;
            GetXY(worldPosition, out x, out y, gridspace);
            return GetValue(x, y);
        }

        private void GetXY(Vector3 worldPosition, out int x, out int y, float gridspace = 1)
        {
            x = Mathf.FloorToInt((worldPosition).x  / (cellSize * gridspace));
            y = Mathf.FloorToInt((worldPosition).y / (cellSize * gridspace));
        }

        private Vector3 GetWorldPosition(int x, int y, float gridspace = 1)
        {
            return new Vector3(x * gridspace, y * gridspace) * cellSize;
        }

        // Create Cube in the World
        private GameObject CreateCube(string text, Color color, Transform parent = null, Vector3 localPosition = default(Vector3), Vector3 localScale = default(Vector3), int fontSize = 40)
        {
            if (color == null) color = Color.white;
            return CreateCube(parent, text, localPosition, localScale, fontSize, color);
        }

        // Create Cube in the World
        private GameObject CreateCube(Transform parent, string text, Vector3 localPosition, Vector3 localScale, int fontSize, Color color)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.name = text;
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            localPosition.z = localScale.z / 2;
            transform.localPosition = localPosition;
            transform.localScale = localScale;
            Renderer rend = gameObject.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Standard"));

            Quaternion localRotation = Quaternion.Euler(0f,0f,0f);

           rend.material.color = color;

            TextMesh textMesh = CreateWorldText(text, gameObject.transform, new Vector3(0f,0f,0.5f), localRotation, fontSize, Color.black, TextAnchor.MiddleCenter, TextAlignment.Left, 5000);
            textMesh.transform.localScale = new Vector3(0.2f,0.2f,1f);
            return gameObject;
        }

        // Create Text in the World+
        private TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3),Quaternion localRotation = default(Quaternion), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
        {
            if (color == null) color = Color.white;
            return CreateWorldText(parent, text, localPosition,localRotation, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
        }

        // Create Text in the World
        private TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition,Quaternion localRotation,int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
        {
            GameObject gameObject = new GameObject(text, typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            return textMesh;
        }
    }
}