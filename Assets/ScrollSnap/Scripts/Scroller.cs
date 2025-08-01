using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

public class Scroller : ScrollRect, IScroller
{

    public CanvasGroup GetCanvasGroup()
    {
        return this.GetComponent<CanvasGroup>();
    }

    public RectTransform GetContentRect()
    {
        return content;
    }

    public RectTransform GetViewportRect()
    {
        return viewport;
    }

    public void AddScrollingListener(UnityAction<Vector2> scrollingListener)
    {
        onValueChanged.AddListener(scrollingListener);
    }

    public void SetVelocity(Vector2 velocity)
    {
        this.velocity = velocity;
    }
}
public interface IScroller
{
    public RectTransform GetContentRect();
    public RectTransform GetViewportRect();
    public CanvasGroup GetCanvasGroup();
    public void SetVelocity(Vector2 velocity);
    public void AddScrollingListener(UnityAction<Vector2> scrollingListener);

}