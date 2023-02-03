using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class CustomGrid
    {
        private int width;
        private int height;
        private int noOfColoumnOforAddInfo;
        string[] dataList;
        private List<double> valueList;
        private float gridZSize = 0.01f;

        private Material material;
        public CustomGrid(GameObject parentObject,float m_cellSize, string[] dataList,int noOfColoumnOforAddInfo)
        {
            GridArray.cellSize = m_cellSize;
            this.dataList = dataList;
            this.noOfColoumnOforAddInfo = noOfColoumnOforAddInfo;

            width = dataList[0].Split(MapInfo.fieldSeperator).Length; // Number of coloumn in csv file
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
                string[] data = dataList[x].Split(MapInfo.fieldSeperator);

                for (int y = 1; y < width - noOfColoumnOforAddInfo; y++)
                {
                    if (x != 0 && y != 0)
                    {
                        var intVal = 0;
                        double value = 0;
                        string fData = data[y];

                        if(int.TryParse(fData,out intVal))
                        {
                            value = intVal;
                        }

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
            List<string> infoHeaderList = new List<string>();
            int infoDataIndex = width - noOfColoumnOforAddInfo;
            for (int x = 0; x < height; x++)
            {
                string[] data = dataList[x].Split(MapInfo.fieldSeperator);
                List<string> infoValueList = new List<string>();

                for (int y = 0; y < width; y++)
                {
                    // Removing Header
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    GameObject cubeParentObject = null;
                    GameObject cubeObject = null;
                    if (y < infoDataIndex)
                    {
                        cubeParentObject = new GameObject("Parent" + x + y);
                        cubeParentObject.transform.SetParent(parentObject.transform, false);

                        cubeObject = new GameObject("Child" + x + y);
                        cubeObject.transform.SetParent(cubeParentObject.transform, false);
                    }
                    Vector3 textPos = new Vector3(0f, 0f, -0.515f);

                    if (x == 0 && y != 0)
                    {
                        if(y >= infoDataIndex)
                        {
                            if (data[y].Contains("\r"))
                            {
                                data[y] = data[y].Replace("\r", string.Empty);
                            }
                            infoHeaderList.Add(data[y]);
                        }
                        else
                        {
                            Vector3 tectPosLabel = textPos;
                            tectPosLabel.x = -1.2f;
                            CreateCubeWithText(data[y], new Color(1f, 1f, 1f, 0f), Color.white, cubeObject.transform, GetWorldPosition(x, y), tectPosLabel, new Vector3(GridArray.cellSize, GridArray.cellSize, gridZSize), new Vector3(0.2f, 0.2f, 1f), Quaternion.Euler(0f, 0f, 0f), true, 20, TextAnchor.MiddleRight, TextAlignmentOptions.Right);
                        }
                    }
                    else if (x != 0 && y != 0)
                    {
                        if (y < infoDataIndex){
                            var intVal = 0;
                            double value = 0f;
                            string fData = data[y];

                            if (int.TryParse(fData, out intVal))
                            {
                                value = intVal;
                            }
                            if (!double.TryParse(fData, out value))
                            {
                                value = 0f;
                            }

                            Vector3 worldPosition = GetWorldPosition(x, y);
                            SetValue(worldPosition, value);
                            string displayvalue = value.ToString();
                            if (displayvalue.Contains("."))
                            {
                                int length = displayvalue.Substring(displayvalue.IndexOf(".")).Length;
                                if (length > 2)
                                {
                                    displayvalue = value.ToString("0.00");
                                }
                            }

                            Color cubeColor = colorHeatMap.GetColorForValue(value, valueList.Min(), valueList.Max());
                            GameObject infoParent = CreateCubeWithText(displayvalue, cubeColor, Color.black, cubeObject.transform, worldPosition, textPos, new Vector3(GridArray.cellSize, GridArray.cellSize, gridZSize), new Vector3(0.2f, 0.2f, 1f), Quaternion.Euler(0f, 0f, 00f), false, 20, TextAnchor.MiddleCenter, TextAlignmentOptions.Center);

                            if (infoValueList.Count == 0 && infoHeaderList.Count != 0)
                            {
                                for (int i = 0; i < noOfColoumnOforAddInfo; i++)
                                {
                                    string text = infoHeaderList[i] + " : " + data[i + infoDataIndex] + System.Environment.NewLine;
                                    infoValueList.Add(text);
                                }
                            }

                            AddInfoDataWithObject(infoParent, infoValueList);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        Vector3 tectPosLabel = textPos;
                        tectPosLabel.y = -1.2f;
                       CreateCubeWithText(data[y], new Color(1f,1f,1f,0f), Color.white, cubeObject.transform, GetWorldPosition(x, y), tectPosLabel, new Vector3(GridArray.cellSize, GridArray.cellSize, gridZSize), new Vector3(0.2f, 0.2f, 1f), Quaternion.Euler(0f, 0f, 90f), true,20, TextAnchor.MiddleLeft, TextAlignmentOptions.Right);
                    }
                }
            }
        }

        private void AddInfoDataWithObject(GameObject infoParent, List<string> list)
        {
            InfoData infoDataObj = infoParent.AddComponent<InfoData>();
           infoDataObj.infoDataList = list;
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

        public void SetValue(Vector3 worldPosition, double value)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
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

        public static double GetValue(Vector3 worldPosition)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return GetValue(x, y);
        }

        private static void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition).x  / (GridArray.cellSize));
            y = Mathf.FloorToInt((worldPosition).y / (GridArray.cellSize));
        }

        private Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * GridArray.cellSize;
        }

        // Create Cube with text in the World
        private GameObject CreateCubeWithText(string text, Color Cubecolor,Color fontColor, Transform parent = null, Vector3 cubePosition = default(Vector3), Vector3 textPosition = default(Vector3),Vector3 CubeScale = default(Vector3), Vector3 textScale = default(Vector3), Quaternion textRotation = default(Quaternion), bool isOnlyText = false, int fontSize = 20, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignmentOptions textAlignment = TextAlignmentOptions.Left)
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

            if (isOnlyText)
            {
                gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                gameObject.tag = GridArray.cubeTag;
            }

            CreateWorldText(text, fontColor, gameObject.transform, textPosition, textRotation, textScale, isOnlyText, fontSize, textAnchor, textAlignment);

            return gameObject;
        }

        // Create Text in the World
        private TextMeshPro CreateWorldText(string text, Color fontColor, Transform parent = null, Vector3 localPosition = default(Vector3), Quaternion localRotation = default(Quaternion), Vector3 textScale = default(Vector3), bool isOnlyText = false,int fontSize = 20, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignmentOptions textAlignment = TextAlignmentOptions.Left, int sortingOrder = 5000)
        {
            GameObject gameObject = new GameObject("Value" +text, typeof(TextMeshPro));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            transform.localScale = textScale;
            TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.autoSizeTextContainer = true;
            textMesh.enableAutoSizing = true;
            textMesh.fontSizeMax = 12;
            textMesh.fontSizeMin = 2;
            textMesh.fontSize = fontSize;
            textMesh.color = fontColor;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            return textMesh;
        }
    }
}