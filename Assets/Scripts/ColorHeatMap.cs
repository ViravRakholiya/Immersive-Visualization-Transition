using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorHeatMap
{
    public byte Alpha = 0xff;
    public List<Color> ColorsOfMap = new List<Color>();

    private int noOfFraction;
    private double minVal;
    private double maxVal;
    private List<double> fractionValueList;


    public ColorHeatMap()
    {
        initColorsBlocks();
    }

    public ColorHeatMap(double minVal, double maxVal,int noOfFraction)
    {
        this.minVal = minVal;
        this.maxVal = maxVal;
        this.noOfFraction = noOfFraction;
        fractionValueList = new List<double>();

        initColorsBlocks();
        CreateFractionForColorPalate();
    }

    public ColorHeatMap(byte alpha)
    {
        this.Alpha = alpha;
        initColorsBlocks();
    }
    private void initColorsBlocks()
    {
        ColorsOfMap.AddRange(new Color[]{
            new Color(0, 71, 109),
            new Color(21, 85, 126) ,
            new Color(37, 100, 143) ,
            new Color(51, 114, 161) ,
            new Color(65, 130, 179) ,
            new Color(78, 145, 198) ,
            new Color(92, 161, 217) ,
            new Color(105, 177, 236), 
            new Color(119, 194, 255)
        });
     }

     public Color GetColorForValue(double val, double minVal, double maxVal)
     {
         Color heatMapColor = new Color();
         var value = (val - minVal) / (maxVal - minVal);
         for (int i = 0; i < noOfFraction; i++)
         {
             if(val <= fractionValueList[i])
             {
                 heatMapColor = GetColorFromTwoFixedColors(value, ColorsOfMap[i], ColorsOfMap[i+1]);
                 break;
             }

             if (i == noOfFraction-1 && val > maxVal) 
             {
                 heatMapColor = GetColorFromTwoFixedColors(value, ColorsOfMap[i], ColorsOfMap[i + 1]);
             }
         }

        return heatMapColor;    
    }

    private void CreateFractionForColorPalate()
    {
        double fractionPoint = (maxVal - minVal) / noOfFraction;
        fractionValueList.Add(minVal);
        for (int i = 1; i < noOfFraction; i++)
        {
                fractionValueList.Add(fractionValueList[i-1] + fractionPoint);
        }
    }
    private Color GetColorFromTwoFixedColors(double value,Color minColor,Color maxColor)
    {
        /*double colorPerc = 1d / (ColorsOfMap.Count - 1);// % of each block of color. the last is the "100% Color"
        double blockOfColor = value / colorPerc;// the integer part repersents how many block to skip
        int blockIdx = (int)Math.Truncate(blockOfColor);// Idx of 
        double valPercResidual = value - (blockIdx * colorPerc);//remove the part represented of block 
        double percOfColor = valPercResidual / colorPerc;// % of color of this block that will be filled */

        var R = (maxColor.r - minColor.r) * value + minColor.r;      // Evaluated as -255*value + 255.
        var G = (maxColor.g - minColor.g) * value + minColor.g;      // Evaluates as 0.
        var B = (maxColor.b - minColor.b) * value + minColor.b;      // Evaluates as 255*value + 0.

        return new Color((float)R/255.0f, (float)G / 255.0f, (float)B / 255.0f);
    }
}