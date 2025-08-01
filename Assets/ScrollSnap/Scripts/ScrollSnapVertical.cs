using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ElkaGames.UI.Scroll
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScrollRect), typeof(CanvasGroup))]
    public class ScrollSnapVertical : UIBehaviour/*, UnityEngine.EventSystems.IDragHandler*/, IEndDragHandler, IBeginDragHandler
    {
        internal enum Layout
        {
            Horizontal,
            Vertical,
            Grid
        }

        [SerializeField] public int startingIndex = 0;
        [SerializeField] float _snappingSpeed = 2;
        [Range(0, 1)] [SerializeField] float sensitivation = 0.3f;
        [SerializeField] bool _fastSwipe = false;
        [SerializeField] float _swipeTime = 0.3f;
        [SerializeField] float _swipeDelta = 10f;

        [SerializeField] int cellIndex;
        int prevCellIndex = -1 ;
        ScrollRect scrollRect;
        CanvasGroup canvasGroup;
        RectTransform content;
        RectTransform viewport;
        Vector2 cellSize;

        bool isLerping = false;
        float lerpStartTime;
        Vector2 releasedPosition;
        Vector2 targetPosition;
        private float startDragPos;
        public UnityEvent<int> OnItemChanged;
        [SerializeField] private Layout layout;

        protected override void Awake()
        {
            base.Awake();
            cellIndex = startingIndex;
            scrollRect = GetComponent<ScrollRect>();
            canvasGroup = GetComponent<CanvasGroup>();
            content = scrollRect.content;
            viewport = scrollRect.viewport;
            scrollRect.onValueChanged.AddListener(OnDrag);

        }
        protected override void Start()
        {

            content.GetComponent<ScrollSnapAnimation>().Init(viewport, layout);
            // TODO : Make get ItemSize Interface
            this.cellSize = content.GetComponent<ScrollSnapAnimation>().itemSize;
            SetContentSize(content.GetComponent<ScrollSnapAnimation>().ItemCount);
            switch (layout)
            {
                case ScrollSnapHorizontal.Layout.Horizontal:
                    content.anchoredPosition = new Vector2(cellSize.x * cellIndex, content.anchoredPosition.y);
                    _contentMinX = CalculateTargetPoisition(0).x;
                    _contentMaxX = CalculateTargetPoisition(content.GetComponent<ScrollSnapAnimation>().ItemCount - 1).x;
                    break;
                case ScrollSnapHorizontal.Layout.Vertical:
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, -cellSize.y * cellIndex);
                    _contentMinY = CalculateTargetPoisition(0).y;
                    _contentMaxY = CalculateTargetPoisition(content.GetComponent<ScrollSnapAnimation>().ItemCount - 1).y;
                    break;
                case Layout.Grid:
                    throw new NotImplementedException();
                    break;
            }

            if (startingIndex < content.GetComponent<ScrollSnapAnimation>().ItemCount)
            {
                MoveToIndex(startingIndex);
            }
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
                    if(cellIndex != prevCellIndex)  OnItemChanged?.Invoke(cellIndex);
                    prevCellIndex = cellIndex;
                }
            }
        }

        /// <summary>
        /// Needs Cleaning
        /// </summary>
        #region sound related variables 
        private int boldItemIndex = 0;
        private float prevPos;
        [SerializeField] private float offset = .1f;
        private float _contentMinY, _contentMaxY;
        private float _contentMinX, _contentMaxX;
        #endregion
        public void OnBeginDrag(PointerEventData eventData)
        {

            switch (layout)
            {
                case Layout.Horizontal:
                    startDragPos = content.anchoredPosition.x;
                    break;
                case Layout.Vertical:
                    startDragPos = content.anchoredPosition.y;
                    break;
                case Layout.Grid:
                    break;
            }

            boldItemIndex = cellIndex;

        }
        private void OnDrag(Vector2 arg0)
        {

            if (HasIndexSkipped(boldItemIndex))
            {
                switch (layout)
                {
                    case Layout.Horizontal:
                        // detect direction of dragging
                        boldItemIndex += (content.position.x + prevPos) > 0f ? 1 : -1;
                        break;
                    case Layout.Vertical:
                        // detect direction of dragging
                        boldItemIndex += (content.position.y - prevPos) > 0f ? 1 : -1;
                        break;
                    case Layout.Grid:
                        throw new NotImplementedException();
                        break;
                }

                boldItemIndex = Mathf.Clamp(boldItemIndex, 0, content.GetComponent<ScrollSnapAnimation>().ItemCount - 1);

                //TODO : Play SFX
                Debug.Log("Play SFX");
            }
            switch (layout)
            {
                case Layout.Horizontal:
                    //this is for calculating direction of changing postion
                    prevPos = content.position.x;
                    break;
                case Layout.Vertical:
                    //this is for calculating direction of changing postion
                    prevPos = content.position.y;
                    break;
                case Layout.Grid:
                    throw new NotImplementedException();
                    break;
            }


        }
        public void OnEndDrag(PointerEventData data)
        {

            //TODO : Create Coroutine to Invoke 'SnapToIndex(data)' with a '_swipeTime' delay
            if (_fastSwipe)//fast swipe
            {
                bool hasPassedSwipeDelta = false;
                switch (layout)
                {
                    case Layout.Horizontal:
                        hasPassedSwipeDelta = Mathf.Abs(data.delta.x) > _swipeDelta;
                        break;
                    case Layout.Vertical:
                        hasPassedSwipeDelta = Mathf.Abs(data.delta.y) > _swipeDelta;
                        break;
                    case Layout.Grid:
                        throw new NotImplementedException();
                        break;
                }
                if (hasPassedSwipeDelta)
                {
                    Timer.Schedule(this, _swipeTime, () =>
                  {
                      SnapToIndex(content);
                  });
                    return;
                }
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

        private int CalculateScrollingAmount()
        {
            float offset = 0;
            float normalizedOffset = 0;
            int skipping = 0;
            switch (layout)
            {
                case Layout.Horizontal:
                    offset = content.anchoredPosition.x - startDragPos;
                    normalizedOffset = Mathf.Abs(offset / cellSize.x);
                    skipping = (int)Mathf.Floor(normalizedOffset);
                    break;
                case Layout.Vertical:
                    offset = content.anchoredPosition.y - startDragPos;
                    normalizedOffset = Mathf.Abs(offset / cellSize.y);
                    skipping = (int)Mathf.Floor(normalizedOffset);
                    break;
                case Layout.Grid:
                    throw new NotImplementedException();
                    break;
            }

            if (normalizedOffset - skipping > sensitivation)
                return skipping + 1;
            else
            {
                return skipping;
            }
        }
        private Vector2 CalculateTargetPoisition(int index)
        {
            switch (layout)
            {
                case Layout.Horizontal:
                    return new Vector2( -1 * cellSize.x * index + (viewport.rect.xMin), content.anchoredPosition.y);
                case Layout.Vertical:
                    return new Vector2(content.anchoredPosition.x, cellSize.y * index + (viewport.rect.yMax));
                case Layout.Grid:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }

        }
        private bool HasIndexSkipped(int fromIndex)
        {
            switch (layout)
            {
                case Layout.Horizontal:
                    if (content.anchoredPosition.x < _contentMinX || content.anchoredPosition.x > _contentMaxX)
                        return false;
                    else
                        return Vector2.Distance(content.anchoredPosition, CalculateTargetPoisition(fromIndex)) + offset > cellSize.x;
                case Layout.Vertical:
                    if (content.anchoredPosition.y < _contentMinY || content.anchoredPosition.y > _contentMaxY)
                        return false;
                    else
                        return Vector2.Distance(content.anchoredPosition, CalculateTargetPoisition(fromIndex)) + offset > cellSize.y;
                case Layout.Grid:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }

        }

        public void SnapToIndex(PointerEventData data)
        {
            int direction = 0;
            switch (layout)
            {
                case Layout.Horizontal:
                    direction = (data.pressPosition.x - data.position.x) > 0f ? 1 : -1;
                    SnapToIndex(cellIndex + direction * CalculateScrollingAmount());
                    break;
                case Layout.Vertical:
                    direction = (data.position.y - data.pressPosition.y) > 0f ? 1 : -1;
                    SnapToIndex(cellIndex + direction * CalculateScrollingAmount());
                    break;
                case Layout.Grid:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }

        }

        public void SnapToIndex(RectTransform endDragPos)
        {
            int direction = 0;
            switch (layout)
            {
                case Layout.Horizontal:
                    direction = (endDragPos.anchoredPosition.x - startDragPos) > 0f ? 1 : -1;
                    Debug.Log(direction);
                    SnapToIndex(cellIndex + direction * CalculateScrollingAmount());
                    break;
                case Layout.Vertical:
                    direction = (endDragPos.anchoredPosition.y - startDragPos) > 0f ? 1 : -1;
                    Debug.Log(direction);
                    SnapToIndex(cellIndex + direction * CalculateScrollingAmount());
                    break;
                case Layout.Grid:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        public void SnapToIndex(int newCellIndex)
        {
            int maxIndex = content.GetComponent<ScrollSnapAnimation>().ItemCount - 1;
            Debug.Log("newCellIndex : " + newCellIndex);

            newCellIndex = Mathf.Clamp(newCellIndex, 0, maxIndex);
            cellIndex = newCellIndex;

            StartLerping();
        }
        public void MoveToIndex(int newCellIndex)
        {
            int maxIndex = content.GetComponent<ScrollSnapAnimation>().ItemCount;
            if (newCellIndex >= 0 && newCellIndex < maxIndex)
            {
                cellIndex = newCellIndex;
            }
            content.anchoredPosition = CalculateTargetPoisition(cellIndex);
            StartLerping();
        }

        private void StartLerping()
        {
            releasedPosition = content.anchoredPosition;
            targetPosition = CalculateTargetPoisition(cellIndex);
            lerpStartTime = Time.time;
            canvasGroup.blocksRaycasts = false;
            isLerping = true;
        }
        private bool ShouldStopLerping()
        {
            switch (layout)
            {
                case Layout.Horizontal:
                    return Mathf.Abs(content.anchoredPosition.x - targetPosition.x) < 0.1f;
                case Layout.Vertical:
                    return Mathf.Abs(content.anchoredPosition.y - targetPosition.y) < 0.1f;
                case Layout.Grid:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
        private void LerpToElement()
        {
            switch (layout)
            {
                case Layout.Horizontal:
                    float newX = Mathf.Lerp(releasedPosition.x, targetPosition.x, (Time.time - lerpStartTime) * _snappingSpeed);
                    content.anchoredPosition = new Vector2(newX, content.anchoredPosition.y);
                    break;
                case Layout.Vertical:
                    float newY = Mathf.Lerp(releasedPosition.y, targetPosition.y, (Time.time - lerpStartTime) * _snappingSpeed);
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, newY);
                    break;
                case Layout.Grid:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
        private bool IndexShouldChangeFromDrag(PointerEventData data)
        {
            // dragged beyond trigger threshold
            float offset = 0;
            float normalizedOffset = 0;
            switch (layout)
            {
                case Layout.Horizontal:
                    // dragged beyond trigger threshold
                    offset = content.anchoredPosition.x + startDragPos;
                    normalizedOffset = Mathf.Abs(offset / (cellSize.x * 0.5f));
                    break;
                case Layout.Vertical:
                    // dragged beyond trigger threshold
                    offset = content.anchoredPosition.y - startDragPos;
                    normalizedOffset = Mathf.Abs(offset / (cellSize.y * 0.5f));
                    break;
                case Layout.Grid:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }

            return normalizedOffset >= 1;
        }

        private void SetContentSize(int elementCount)
        {
            RectOffset padding = content.GetComponent<HorizontalOrVerticalLayoutGroup>().padding;
            switch (layout)
            {
                case Layout.Horizontal:
                    content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cellSize.x * elementCount + viewport.rect.width);
                    padding.left = (int)(viewport.rect.width * (3f / 4));
                    padding.right = (int)(viewport.rect.width * (1f / 4));
                    content.GetComponent<HorizontalOrVerticalLayoutGroup>().padding = padding;
                    break;
                case Layout.Vertical:
                    content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cellSize.y * elementCount + viewport.rect.height);
                    padding.bottom = (int)(viewport.rect.height * (3f / 4));
                    padding.top = (int)(viewport.rect.height * (1.3f / 4));
                    content.GetComponent<HorizontalOrVerticalLayoutGroup>().padding = padding;
                    break;
                case Layout.Grid:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        public ScrollSnapItem GetListItem(int index)
        {
            return content.GetComponent<ScrollSnapAnimation>().GetItem(index);
        }

        public List<ScrollSnapItem> GetListAllItems()
        {
            return content?.GetComponent<ScrollSnapAnimation>().GetAllItems();
        }
    }
}