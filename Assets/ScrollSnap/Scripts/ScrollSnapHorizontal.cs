using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ElkaGames.UI.Scroll
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Scroller), typeof(CanvasGroup))]
    public class ScrollSnapHorizontal : IScrollSnap
    {

        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();
        }
        private void LateUpdate()
        {
            if (scrollLerpData.isLerping)
            {
                LerpToElement();
                if (ShouldStopLerping())
                {
                    scrollLerpData.isLerping = false;
                    scroller.GetCanvasGroup().blocksRaycasts = true;
                    scroller.SetVelocity( Vector2.zero);
                    if (cellIndex != prevCellIndex) OnItemChanged?.Invoke(cellIndex);
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
        public override void OnBeginDrag(PointerEventData eventData)
        {
            scrollLerpData = new ScrollLerpData(scroller.GetContentRect());
            boldItemIndex = cellIndex;
        }
        public override void OnDrag(Vector2 arg0)
        {

            if (HasIndexSkipped(boldItemIndex))
            {

                // detect direction of dragging
                boldItemIndex += (scroller.GetContentRect().position.x + prevPos) > 0f ? 1 : -1;

                boldItemIndex = Mathf.Clamp(boldItemIndex, 0, scroller.GetContentRect().GetComponent<ScrollSnapAnimation>().ItemCount - 1);

                //TODO : Play SFX
                Debug.Log("Play SFX");
            }

            //this is for calculating direction of changing postion
            prevPos = scroller.GetContentRect().position.x;


        }
        public override void OnEndDrag(PointerEventData data)
        {

            if (_fastSwipe && Mathf.Abs(data.delta.x) > _swipeDelta)//fast swipe
            {
                Timer.Schedule(this, _swipeTime, () =>
                    {
                        SnapToIndex(scroller.GetContentRect());
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

        protected override int CalculateScrollingAmount()
        {
            float offset = scroller.GetContentRect().anchoredPosition.x - scrollLerpData.startDragPos.x;
            float normalizedOffset = Mathf.Abs(offset / cellSize.x);
            int skipping = (int)Mathf.Floor(normalizedOffset);

            if (normalizedOffset - skipping > sensitivation)
                return skipping + 1;
            else
            {
                return skipping;
            }
        }

        protected override Vector2 CalculateTargetPoisition(int index)
        {
            return new Vector2(-1 * cellSize.x * index + (scroller.GetViewportRect().rect.xMin), scroller.GetContentRect().anchoredPosition.y);
        }

        protected override bool HasIndexSkipped(int fromIndex)
        {

            if (scroller.GetContentRect().anchoredPosition.x < _contentMinX || scroller.GetContentRect().anchoredPosition.x > _contentMaxX)
                return false;
            else
                return Vector2.Distance(scroller.GetContentRect().anchoredPosition, CalculateTargetPoisition(fromIndex)) + offset > cellSize.x;


        }

        protected override void SnapToIndex(PointerEventData data)
        {
            int direction = 0;

            direction = (data.pressPosition.x - data.position.x) > 0f ? 1 : -1;
            SnapToIndex(cellIndex + direction * CalculateScrollingAmount());

        }

        protected override void SnapToIndex(RectTransform endDragPos)
        {
            int direction = 0;

            direction = (endDragPos.anchoredPosition.x - scrollLerpData.startDragPos.x) > 0f ? 1 : -1;
            Debug.Log(direction);
            SnapToIndex(cellIndex + direction * CalculateScrollingAmount());
        }

        protected override void SnapToIndex(int newCellIndex)
        {
            int maxIndex = scroller.GetContentRect().GetComponent<ScrollSnapAnimation>().ItemCount - 1;
            Debug.Log("newCellIndex : " + newCellIndex);

            newCellIndex = Mathf.Clamp(newCellIndex, 0, maxIndex);
            cellIndex = newCellIndex;

            StartLerping();
        }
        protected override void MoveToIndex(int newCellIndex)
        {
            int maxIndex = scroller.GetContentRect().GetComponent<ScrollSnapAnimation>().ItemCount;
            if (newCellIndex >= 0 && newCellIndex < maxIndex)
            {
                cellIndex = newCellIndex;
            }
            scroller.GetContentRect().anchoredPosition = CalculateTargetPoisition(cellIndex);
            StartLerping();
        }

        protected override void StartLerping()
        {
            scrollLerpData.releasedPosition = scroller.GetContentRect().anchoredPosition;
            scrollLerpData.targetPosition = CalculateTargetPoisition(cellIndex);
            scrollLerpData.lerpStartTime = Time.time;
            scroller.GetCanvasGroup().blocksRaycasts = false;
            scrollLerpData.isLerping = true;
        }

        protected override bool ShouldStopLerping()
        {
            return Mathf.Abs(scroller.GetContentRect().anchoredPosition.x - scrollLerpData.targetPosition.x) < 0.1f;
        }

        protected override void LerpToElement()
        {
            float newX = Mathf.Lerp(scrollLerpData.releasedPosition.x, scrollLerpData.targetPosition.x, (Time.time - scrollLerpData.lerpStartTime) * _snappingSpeed);
            scroller.GetContentRect().anchoredPosition = new Vector2(newX, scroller.GetContentRect().anchoredPosition.y);
        }
        protected override bool IndexShouldChangeFromDrag(PointerEventData data)
        {           
            // dragged beyond trigger threshold
            float offset = scroller.GetContentRect().anchoredPosition.x + scrollLerpData.startDragPos.x;
            float normalizedOffset = Mathf.Abs(offset / (cellSize.x * 0.5f));

            return normalizedOffset >= 1;
        }

        protected override void SetContentSize(int elementCount)
        {
            RectTransform content = scroller.GetContentRect();
            RectTransform viewport = scroller.GetViewportRect();

            RectOffset padding = content.GetComponent<HorizontalOrVerticalLayoutGroup>().padding;

            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cellSize.x * elementCount + viewport.rect.width);
            padding.left = (int)(viewport.rect.width * (3f / 4));
            padding.right = (int)(viewport.rect.width * (1f / 4));
            content.GetComponent<HorizontalOrVerticalLayoutGroup>().padding = padding;
        }

        public ScrollSnapItem GetListItem(int index)
        {
            return scroller.GetContentRect().GetComponent<ScrollSnapAnimation>().GetItem(index);
        }

        public List<ScrollSnapItem> GetListAllItems()
        {
            return scroller.GetContentRect().GetComponent<ScrollSnapAnimation>().GetAllItems();
        }

        protected override void SetScrollItemSize()
        {
            this.cellSize = new Vector2(100, 100);
        }

        protected override void SetContentPosition()
        {

            scroller.GetContentRect().anchoredPosition = new Vector2(cellSize.x * cellIndex, scroller.GetContentRect().anchoredPosition.y);
            _contentMinX = CalculateTargetPoisition(0).x;
            _contentMaxX = CalculateTargetPoisition(scroller.GetContentRect().GetComponent<ScrollSnapAnimation>().ItemCount - 1).x;
        }

       
    }
}