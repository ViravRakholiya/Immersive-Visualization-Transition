using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorHeatMap
{
    public ColorHeatMap()
    {
        initColorsBlocks();
    }
    public ColorHeatMap(byte alpha)
    {
        this.Alpha = alpha;
        initColorsBlocks();
    }
    private void initColorsBlocks()
    {
        ColorsOfMap.AddRange(new Color[]{
            new Color(0, 0, 0,Alpha) ,//Black
            new Color( 0, 0, 0xFF,Alpha) ,//Blue
            new Color(0, 0xFF, 0xFF,Alpha) ,//Cyan
            new Color(0, 0xFF, 0,Alpha) ,//Green
            new Color(0xFF, 0xFF, 0,Alpha) ,//Yellow
            new Color(0xFF, 0, 0,Alpha) ,//Red
            new Color(0xFF, 0xFF, 0xFF,Alpha) // White
        });
    }
    public Color GetColorForValue(double val, double minVal, double maxVal)
    {
        double valPerc = val / maxVal;// value%
        double colorPerc = 1d / (ColorsOfMap.Count - 1);// % of each block of color. the last is the "100% Color"
        double blockOfColor = valPerc / colorPerc;// the integer part repersents how many block to skip
        int blockIdx = (int)Math.Truncate(blockOfColor);// Idx of 
        double valPercResidual = valPerc - (blockIdx * colorPerc);//remove the part represented of block 
        double percOfColor = valPercResidual / colorPerc;// % of color of this block that will be filled

        Color cTarget = ColorsOfMap[blockIdx];
        Color cNext = val == maxVal ? ColorsOfMap[blockIdx] : ColorsOfMap[blockIdx + 1];

        var deltaR = cNext.r - cTarget.r;
        var deltaG = cNext.g - cTarget.g;
        var deltaB = cNext.b - cTarget.b;

        var R = cTarget.r + (deltaR * percOfColor);
        var G = cTarget.g + (deltaG * percOfColor);
        var B = cTarget.b + (deltaB * percOfColor);

        Color c = ColorsOfMap[0];
        try
        {
            c = new Color((byte)R, (byte)G, (byte)B, Alpha);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        return c;
    }
    public byte Alpha = 0xff;
    public List<Color> ColorsOfMap = new List<Color>();
}