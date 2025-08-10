using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Alec.UI.Scroll
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScrollRect), typeof(CanvasGroup))]
    public class GridScrollSnap : UIBehaviour, IEndDragHandler,IBeginDragHandler
    {

        [SerializeField] public int startingIndex = 0;
        [SerializeField] float _lerpMultiplier = 2;
        [Range(0, 1)] [SerializeField] float sensitivation = 0.3f;


        [SerializeField] bool _fastSwipe = false;
        [SerializeField] float _swipeTime = 0.3f;
        [SerializeField] float _swipeDelta = 10f;

        [SerializeField]int cellIndex;
        int cellsCount;
        ScrollRect scrollRect;
        CanvasGroup canvasGroup;
        RectTransform content;
        [SerializeField] ListGrid thumbsGrid;
        RectTransform viewport;
        [SerializeField] Vector2 cellSize;

        bool isLerping = false;
        float lerpStartedAt;
        Vector2 targetPosition;

        public event Action<int> OnItemChanged;

        protected override void Awake()
        {
            base.Awake();
            cellIndex = startingIndex;
            scrollRect = GetComponent<ScrollRect>();
            canvasGroup = GetComponent<CanvasGroup>();
            content = scrollRect.content;
            viewport = scrollRect.viewport;
        }
        protected override void Start()
        {
            cellsCount = thumbsGrid.itemsMap.Count;
            cellsCount = 100;

            //if (startingIndex < cellsCount)
            //{
            //    SnapToIndex(startingIndex);
            //}
        }
        private void LateUpdate()
        {
            if (isLerping)
            {
                LerpToElement();
                if (ShouldStopLerping())
                {
                    isLerping = false;
                    canvasGroup.blocksRaycasts = true;
                    scrollRect.velocity = Vector2.zero;
                    OnItemChanged?.Invoke(cellIndex);
                }
            }
        }

        Vector2 _startPos;
        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPos = content.anchoredPosition;
        }

        public void OnEndDrag(PointerEventData data)
        {
            float draggedDelta = Vector2.Distance(content.anchoredPosition ,_startPos);
            //TODO : Create Coroutine to Invoke 'SnapToIndex(data)' with a '_swipeTime' delay
            if (_fastSwipe && draggedDelta > _swipeDelta)//fast swipe
            {

                Timer.Schedule(this, _swipeTime, () =>
              {
                  SnapToItem(thumbsGrid.GetBoldItem());
              });
                return;

            }

            if (IndexShouldChangeFromDrag(draggedDelta))
            {
                SnapToItem(thumbsGrid.GetBoldItem());
            }
            else
            {
                StartLerping();
            }
        }

        private Vector2 CalculateScrollingAmount(int index)
        {
            return thumbsGrid.GetItemOffsetFromBold(index);
        }

        public void SnapToItem(GridListItem boldItem)
        {
            cellIndex = Mathf.Clamp(boldItem.index, 0, cellsCount - 1);
            StartLerping();
        }

        public void SnapToIndex(int newCellIndex)
        {
            cellIndex = Mathf.Clamp(newCellIndex, 0, cellsCount - 1);
            StartLerping();
        }
        public int x = 100;
        private void StartLerping()
        {
            Vector2 offset = CalculateScrollingAmount(cellIndex);
            targetPosition = content.anchoredPosition + offset;
            lerpStartedAt = Time.time;
            canvasGroup.blocksRaycasts = false;
            isLerping = true;
        }
        private bool ShouldStopLerping()
        {
            return Vector2.Distance(content.anchoredPosition, targetPosition) < 0.1f;
        }
        private void LerpToElement()
        {
            content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, targetPosition, (lerpStartedAt) * _lerpMultiplier);
        }
        private bool IndexShouldChangeFromDrag(float draggedDelta)
        {
            // dragged beyond trigger threshold
            float offset = draggedDelta;
            float normalizedOffset = 0;

            // dragged beyond trigger threshold
            normalizedOffset = Mathf.Abs(offset / (cellSize.magnitude * 0.5f));


            return normalizedOffset >= 1;
        }

    }

    public class GridListItem
    {
        internal int index;
    }

    internal class ListGrid
    {
        internal Dictionary<int,GridListItem> itemsMap;

        internal GridListItem GetBoldItem()
        {
            throw new NotImplementedException();
        }

        internal Vector2 GetItemOffsetFromBold(int index)
        {
            throw new NotImplementedException();
        }
    }
}