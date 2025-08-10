using Alec.UI.Scroll;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface IScrollSnapCalculator 
{
    public void Initalize(RectTransform centerObject, Vector2 cellSize, RectTransform content, RectTransform viewport, int elementsCount, float alienOffset);
    public float CalculateLerpValue(ScrollSnapItem item);
    public float CalculateDeltaPosition(PointerEventData data);
    public void SetScrollingData(ScrollingData scrollingData);
    public int CalculateScrollingAmount(float sensitivation);
    public Vector2 CalculateTargetPoisition(int index);
    public int GetScrollDirection(PointerEventData data);
    public int GetScrollDirection(Vector2 startDragPos, Vector2 endDragPos);
    public void LerpToElement(float snappingSpeed);
    public void SetContentSize(int elementCount);
    public bool ShouldStopLerping();


}
public abstract class ScrollSnapCalculator : ScriptableObject, IScrollSnapCalculator
{
    protected RectTransform _centerObject;
    protected float _itemDistanceFromCenterObject;
    protected Vector2 _cellSize;
    protected RectTransform _content;
    protected RectTransform _viewport;
    protected HorizontalOrVerticalLayoutGroup _layout;
    protected int _elementsCount;
    protected float _alienOffset;
    protected ScrollingData _scrollingData;
    public virtual void Initalize(RectTransform centerObject, Vector2 cellSize, RectTransform content, RectTransform viewport, int elementsCount, float alienOffset)
    {
        _centerObject = centerObject;
        _cellSize = cellSize;
        _viewport = viewport;
        _content = content;
        _layout = content.GetComponent<HorizontalOrVerticalLayoutGroup>();
        _elementsCount = elementsCount;
        _alienOffset = alienOffset;
    }
    public abstract float CalculateDeltaPosition(PointerEventData data);
    public abstract float CalculateLerpValue(ScrollSnapItem item);
    public abstract int CalculateScrollingAmount(float sensitivation);
    public abstract Vector2 CalculateTargetPoisition(int index);
    public abstract int GetScrollDirection(PointerEventData data);
    public abstract int GetScrollDirection(Vector2 startDragPos, Vector2 endDragPos);

    public abstract void LerpToElement(float snappingSpeed);
    public abstract void SetContentSize(int elementCount);
    public abstract void SetScrollingData(ScrollingData scrollingData);
    public abstract bool ShouldStopLerping();
}