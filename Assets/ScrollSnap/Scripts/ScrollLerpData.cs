using System;
using UnityEngine;

public class ScrollLerpData 
{
    public ScrollLerpData(RectTransform scrollContent)
    {
        startDragPos = scrollContent.anchoredPosition;
    }
    internal bool isLerping = false;
    internal float lerpStartTime;
    internal Vector2 releasedPosition;
    internal Vector2 targetPosition;
    internal Vector2 startDragPos;

}
