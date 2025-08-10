using System;
using UnityEngine;

public class ScrollingData
{
    public ScrollingData(RectTransform scrollContent)
    {
        if (scrollContent != null)
            startDragPos = scrollContent.anchoredPosition;
    }
    internal bool isLerping = false;
    internal float lerpStartTime;
    internal Vector2 releasedPosition;
    internal Vector2 targetPosition;
    internal Vector2 startDragPos;

}
