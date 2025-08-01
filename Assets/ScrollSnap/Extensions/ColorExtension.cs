using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension
{
    public static Color SetAlpha(this Color color,float newAlpha = 0)
    {
        color.a = newAlpha;
        return color;
    }
}
