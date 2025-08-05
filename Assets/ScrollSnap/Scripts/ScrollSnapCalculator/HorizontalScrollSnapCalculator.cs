using ElkaGames.UI.Scroll;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "HorizontalScrollSnapCalculator", menuName = "Scroll Snap/Horizontal Calculator")]
public class HorizontalScrollSnapCalculator : ScrollSnapCalculator
{
  
    public override float CalculateLerpValue(ScrollSnapItem item)
    {
        _itemDistanceFromCenterObject = Mathf.Abs(_centerObject.position.x - item.mRect.position.x);
        return _cellSize.x * 0.6f / (_itemDistanceFromCenterObject);
    }
    public override float CalculateDeltaPosition(PointerEventData data)
    {
        return Mathf.Abs(data.scrollDelta.x);
    }
    public override void SetScrollingData(ScrollingData scrollingData)
    {
        _scrollingData = scrollingData;
    }
    public override int CalculateScrollingAmount(float sensitivation)
    {
        float offset = _content.anchoredPosition.x - _scrollingData.startDragPos.x;
        float normalizedOffset = Mathf.Abs(offset / _cellSize.x);
        int skipping = (int)Mathf.Floor(normalizedOffset);

        if (normalizedOffset - skipping > sensitivation)
            return skipping + 1;
        else
        {
            return skipping;
        }
    }

    public override Vector2 CalculateTargetPoisition(int index)
    {
        index = Mathf.Clamp(index, 0, _elementsCount - 1);
        return new Vector2(-1 * _cellSize.x * index - _layout.spacing * index - _alienOffset * index - _cellSize.x / 2, _content.anchoredPosition.y);
    }

    public override int GetScrollDirection(PointerEventData data)
    {
        int direction = 0;
        direction = (data.pressPosition.x - data.position.x) > 0f ? 1 : -1;
        return direction;
    }

    public override int GetScrollDirection(Vector2 startDragPos, Vector2 endDragPos)
    {
        int direction = 0;
        direction = (startDragPos.x - endDragPos.x) > 0f ? 1 : -1;
        return direction;
    }

    
    public override void LerpToElement(float snappingSpeed)
    {
        float newX = Mathf.Lerp(_scrollingData.releasedPosition.x, _scrollingData.targetPosition.x, (Time.time - _scrollingData.lerpStartTime) * snappingSpeed);
        _content.anchoredPosition = new Vector2(newX, _content.anchoredPosition.y);
    }

    public override void SetContentSize(int elementCount)
    {        
        _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _cellSize.x * elementCount + _layout.spacing * elementCount + _viewport.rect.width);
        _layout.padding.left = (int)(_viewport.rect.width * (2f / 4));
        _layout.padding.right = (int)(_viewport.rect.width * (2f / 4));
        _content.GetComponent<HorizontalOrVerticalLayoutGroup>().padding = _layout.padding;
    }

    public override bool ShouldStopLerping()
    {
        return Mathf.Abs(_content.anchoredPosition.x - _scrollingData.targetPosition.x) < 0.1f;
    }

}
