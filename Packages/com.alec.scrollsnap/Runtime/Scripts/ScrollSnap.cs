using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Alec.UI.Scroll
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScrollRect), typeof(CanvasGroup))]
    public class ScrollSnap : UIBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] protected int startingIndex = 0;
        [SerializeField] protected float _snappingSpeed = 2;
        [Range(0, 1)][SerializeField] protected float sensitivation = 0.3f;

        [SerializeField] protected bool _fastSwipe = false;
        [SerializeField] protected float _swipeTime = 0.3f;
        [SerializeField] protected float _swipeDelta = 10f;
        [SerializeField] protected int cellIndex;
        protected int prevCellIndex = -1;


        [SerializeField] protected ScrollSnapCalculator calculator;
        [SerializeField] protected ScrollSnapItemPoolSO scrollingItemPool;
        protected List<ScrollSnapItem> itemsList;

        [SerializeField] protected RectTransform _centerObject;
        protected float itemDistanceFromCenterObject;


        protected Vector2 cellSize;

        protected ScrollingData scrollLerpData = new ScrollingData(default);
        ScrollRect _scrollRect;
        CanvasGroup _canvasGroup;

        public UnityEvent<int> OnItemChanged;
        [SerializeField] float alienOffset = 3.5f;
        protected override void Awake()
        {
            base.Awake();
            cellIndex = startingIndex;
            _scrollRect = GetComponent<ScrollRect>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        protected override void Start()
        {
            InstantiateScrollingItems();
            SetScrollItemSize();
            calculator.Initalize(_centerObject, cellSize, GetComponent<ScrollRect>().content, GetComponent<ScrollRect>().viewport, itemsList.Count, alienOffset);
            SetContentSize(itemsList.Count);
            MoveToIndex(startingIndex);
        }
        protected virtual void LateUpdate()
        {
            if (scrollLerpData.isLerping)
            {
                LerpToElement();
                if (ShouldStopLerping())
                {
                    scrollLerpData.isLerping = false;
                    _canvasGroup.blocksRaycasts = true;
                    _scrollRect.velocity = Vector2.zero;
                    if (cellIndex != prevCellIndex) OnItemChanged?.Invoke(cellIndex);
                    prevCellIndex = cellIndex;
                }
            }

            for (int i = 0; i < itemsList.Count; i++)
            {
                itemsList[i].Adapt(CalculateLerpValue(i));
            }
        }
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            scrollLerpData = new ScrollingData(_scrollRect.content);
            calculator.SetScrollingData(scrollLerpData);
        }
        protected float CalculateLerpValue(int itemIndex)
        {
            return calculator.CalculateLerpValue(itemsList[itemIndex]);
        }
        protected void InstantiateScrollingItems()
        {
            scrollingItemPool.OverridePoolRoot(_scrollRect.content);
            itemsList = scrollingItemPool.Request(50) as List<ScrollSnapItem>;

            for (int i = 0; i < itemsList.Count; i++)
            {
                itemsList[i].transform.localScale = Vector3.one;
            }

        }
        public void OnEndDrag(PointerEventData data)
        {

            if (_fastSwipe && calculator.CalculateDeltaPosition(data) > _swipeDelta)//fast swipe
            {
                Timer.Schedule(this, _swipeTime, () =>
                    {
                        SnapToIndex(_scrollRect.content);
                    });
                return;
            }

            if (IndexShouldChangeFromDrag(data))
            {
                SnapToIndex(data);
            }
            else
            {
                StartLerping();
            }
        }
        protected void SnapToIndex(PointerEventData data)
        {
            int direction = calculator.GetScrollDirection(data);
            SnapToIndex(cellIndex + direction * CalculateScrollingAmount());

        }
        protected void SnapToIndex(RectTransform endDragPos)
        {
            int direction = calculator.GetScrollDirection(scrollLerpData.startDragPos, endDragPos.anchoredPosition);
            SnapToIndex(cellIndex + direction * CalculateScrollingAmount());
        }

        protected int CalculateScrollingAmount()
        {
            return calculator.CalculateScrollingAmount(sensitivation);
        }
        protected void SnapToIndex(int newCellIndex)
        {
            cellIndex = Mathf.Clamp(newCellIndex, 0, itemsList.Count - 1);
            Debug.Log("newCellIndex : " + cellIndex);

            StartLerping();
        }
        protected void MoveToIndex(int newCellIndex)
        {
            cellIndex = Mathf.Clamp(newCellIndex, 0, itemsList.Count - 1);
            _scrollRect.content.anchoredPosition = CalculateTargetPoisition(cellIndex);
        }
        protected void StartLerping()
        {
            if (scrollLerpData == null) scrollLerpData = new ScrollingData(_scrollRect.content);
            scrollLerpData.releasedPosition = _scrollRect.content.anchoredPosition;
            scrollLerpData.targetPosition = CalculateTargetPoisition(cellIndex);
            scrollLerpData.lerpStartTime = Time.time;
            _canvasGroup.blocksRaycasts = false;
            scrollLerpData.isLerping = true;
            calculator.SetScrollingData(scrollLerpData);
        }
        protected bool ShouldStopLerping()
        {
            return calculator.ShouldStopLerping();
        }
        protected void LerpToElement()
        {
            calculator.LerpToElement(_snappingSpeed);
        }
        protected bool IndexShouldChangeFromDrag(PointerEventData data)
        {
            return CalculateScrollingAmount() >= 1;
        }
        protected void SetContentSize(int elementCount)
        {
            calculator.SetContentSize(elementCount);
        }
        protected void SetScrollItemSize()
        {
            this.cellSize = scrollingItemPool.Item.GetComponent<RectTransform>().rect.size;
        }


        protected Vector2 CalculateTargetPoisition(int index)
        {
            return calculator.CalculateTargetPoisition(index);
        }

    }
}