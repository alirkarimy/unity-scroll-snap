using Alec.UI.Scroll;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "VerticalScrollSnapCalculator", menuName = "Scroll Snap/Vertical Calculator")]
public class VerticalScrollSnapCalculator : ScrollSnapCalculator
{
   
    public override float CalculateLerpValue(ScrollSnapItem item)
    {
        _itemDistanceFromCenterObject = Mathf.Abs(_centerObject.position.y - item.mRect.position.y);
        return _cellSize.y * 0.2f / (_itemDistanceFromCenterObject);
    }
    public override float CalculateDeltaPosition(PointerEventData data)
    {
        return Mathf.Abs(data.scrollDelta.y);
    }
    public override void SetScrollingData(ScrollingData scrollingData)
    {
        _scrollingData = scrollingData;
    }
    public override int CalculateScrollingAmount(float sensitivation)
    {
        float offset = _content.anchoredPosition.y - _scrollingData.startDragPos.y;
        float normalizedOffset = Mathf.Abs(offset / _cellSize.y);
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
        return new Vector2(_content.anchoredPosition.x , _cellSize.y * index + _layout.spacing * index + _alienOffset * index + _cellSize.y / 2);
    }

    public override int GetScrollDirection(PointerEventData data)
    {
        int direction = 0;
        direction = (data.pressPosition.y - data.position.y) > 0f ? -1 : 1;
        return direction;
    }

    public override int GetScrollDirection(Vector2 startDragPos, Vector2 endDragPos)
    {
        int direction = 0;
        direction = (startDragPos.y - endDragPos.y) > 0f ? -1 : 1;
        return direction;
    }

    
    public override void LerpToElement(float snappingSpeed)
    {
        float newY = Mathf.Lerp(_scrollingData.releasedPosition.y, _scrollingData.targetPosition.y, (Time.time - _scrollingData.lerpStartTime) * snappingSpeed);
        _content.anchoredPosition = new Vector2(_content.anchoredPosition.x,newY);
    }

    public override void SetContentSize(int elementCount)
    {        
        _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _cellSize.y * elementCount + _layout.spacing * elementCount + _viewport.rect.height);
        _layout.padding.top = (int)(_viewport.rect.height * (2f / 4));
        _layout.padding.bottom= (int)(_viewport.rect.height * (2f / 4));
        _content.GetComponent<HorizontalOrVerticalLayoutGroup>().padding = _layout.padding;
    }

    public override bool ShouldStopLerping()
    {
        return Mathf.Abs(_content.anchoredPosition.y - _scrollingData.targetPosition.y) < 0.1f;
    }

}
