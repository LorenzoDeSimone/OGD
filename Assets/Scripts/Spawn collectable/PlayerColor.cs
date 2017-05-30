using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColor
{
    private static float RGBMultiplier = 0.004f;//Multiply [0,255] value with this to obtain value normalized in [0,1]
    private static Color[] color = { new Color(000                , 000                , 254 * RGBMultiplier),       //BLUE
                                     new Color(000                , 254 * RGBMultiplier, 126 * RGBMultiplier),       //GREEN
                                     new Color(254 * RGBMultiplier, 000                , 000                ),       //RED
                                     new Color(255 * RGBMultiplier, 255 * RGBMultiplier, 001 * RGBMultiplier),       //YELLOW
                                   };


    public static Color GetColor(int color_num)
    {
        color_num = Mathf.Abs(color_num);
        return color[color_num % color.Length];
    }
}
