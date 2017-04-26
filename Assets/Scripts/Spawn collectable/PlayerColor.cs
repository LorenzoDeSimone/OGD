using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerColor
{
    private static Color[] color = { new Color(0.45f, 0.75f, 0.45f), new Color(0.45f, 0.45f, 0.75f), new Color(0.75f, 0.45f, 0.45f), new Color(0.75f, 0.45f, 0.75f), new Color(0.75f, 0.75f, 0.45f), new Color(0.45f, 0.75f, 0.75f) };

    public static Color GetColor(int color_num)
    {
        return color[color_num % color.Length];
    }
}
