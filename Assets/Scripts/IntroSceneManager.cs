using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroSceneManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField fileName;
    [SerializeField]
    private TMP_InputField gridSize;
    [SerializeField]
    private TMP_InputField noOfColoumnforAddInfo;
    [SerializeField]
    private TMP_InputField fieldSeperator;
    [SerializeField]
    private Button chooseFile;
    [SerializeField]
    private Button crateHeatMapBtn;

    private string filePath = string.Empty;

    void Awake()
    {
        gridSize.text = MapInfo.gridSize != 0 ? MapInfo.gridSize.ToString() : "0.1";
        noOfColoumnforAddInfo.text = MapInfo.noOfColoumnforAddInfo != 0 ? MapInfo.noOfColoumnforAddInfo.ToString() : "0";
        fieldSeperator.text = MapInfo.fieldSeperator != '\0' ? MapInfo.fieldSeperator.ToString() : ",";
        fileName.text = string.IsNullOrEmpty(MapInfo.filePath) ? "Choose File Name..." : Path.GetFileName(MapInfo.filePath);
        filePath = string.IsNullOrEmpty(MapInfo.filePath) ? string.Empty : MapInfo.filePath;
        gridSize.onDeselect.AddListener(ValidateValue);
        noOfColoumnforAddInfo.onDeselect.AddListener(ValidateValue);
        fieldSeperator.onDeselect.AddListener(ValidateValue);
        crateHeatMapBtn.enabled = !string.IsNullOrEmpty(fileName.text) && fileName.text != "Choose File Name...";
        chooseFile.onClick.AddListener(openfileExplorer);
    }

    private void ValidateValue(string arg0)
    {
        if(string.IsNullOrEmpty(gridSize.text) || string.IsNullOrEmpty(fieldSeperator.text) || string.IsNullOrEmpty(noOfColoumnforAddInfo.text))
        {
            crateHeatMapBtn.enabled = false;
        }
        else
        {
            CreateHeatMapBtnEnable();
        }
    }

    private void openfileExplorer()
    {
        string fileType = NativeFilePicker.ConvertExtensionToFileType("csv");
        if (NativeFilePicker.IsFilePickerBusy())
            return;

        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
                Debug.Log("Operation cancelled");
            else
            {
                filePath = path;
                fileName.text = Path.GetFileName(filePath);
            }
        }, new string[] { fileType });

        if (!string.IsNullOrEmpty(filePath))
        {
            CreateHeatMapBtnEnable();
        }
    }

    public void CreateHeatMapBtnEnable()
    {
        MapInfo.filePath = filePath;
        MapInfo.gridSize = float.Parse(gridSize.text);
        MapInfo.fieldSeperator = char.Parse(fieldSeperator.text);
        MapInfo.noOfColoumnforAddInfo = int.Parse(noOfColoumnforAddInfo.text);
        crateHeatMapBtn.enabled = true;
    }
}
