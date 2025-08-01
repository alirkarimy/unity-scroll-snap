using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ElkaGames.UI.Scroll.IScrollSnap;

namespace ElkaGames.UI.Scroll
{
   

    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScrollRect), typeof(CanvasGroup))]
    public abstract class IScrollSnap : UIBehaviour
    {

        [SerializeField] protected int startingIndex = 0;
        [SerializeField] protected float _snappingSpeed = 2;
        [Range(0, 1)][SerializeField] protected float sensitivation = 0.3f;
       
        [SerializeField] protected bool _fastSwipe = false;
        [SerializeField] protected float _swipeTime = 0.3f;
        [SerializeField] protected float _swipeDelta = 10f;
        [SerializeField] protected int cellIndex;
       

        [SerializeField] protected IScroller scroller;

        protected int prevCellIndex = -1;

        protected Vector2 cellSize;

        protected ScrollLerpData scrollLerpData;

        public UnityEvent<int> OnItemChanged;

        protected override void Awake()
        {
            base.Awake();
            cellIndex = startingIndex;
            scroller = GetComponent<Scroller>();
            scroller.AddScrollingListener(OnDrag);

        }
        protected override void Start()
        {
            scroller.GetContentRect().GetComponent<ScrollSnapAnimation>().Init(viewport, layout);
            SetScrollItemSize();
            SetContentSize(content.GetComponent<ScrollSnapAnimation>().ItemCount);
            SetContentPosition();
            MoveToIndex(startingIndex < content.GetComponent<ScrollSnapAnimation>().ItemCount ? startingIndex : 0);
        }

        protected abstract void SetScrollItemSize();

        protected abstract void SetContentPosition();

        private void LateUpdate()
        {
            if (isLerping)
            {
                LerpToElement();
                if (ShouldStopLerping())
                {
                    isLerping = false;
                    canvasGroup.blocksRaycasts = true;
                    scroller.velocity = Vector2.zero;
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
        public abstract void OnBeginDrag(PointerEventData eventData);
        public abstract void OnDrag(Vector2 arg0);
        public abstract void OnEndDrag(PointerEventData data);

        protected abstract int CalculateScrollingAmount();
        protected abstract Vector2 CalculateTargetPoisition(int index);
        protected abstract bool HasIndexSkipped(int fromIndex);

        protected abstract void SnapToIndex(PointerEventData data);

        protected abstract void SnapToIndex(RectTransform endDragPos);

        protected abstract void SnapToIndex(int newCellIndex);
        protected abstract void MoveToIndex(int newCellIndex);

        protected abstract void StartLerping();
        protected abstract bool ShouldStopLerping();
        protected abstract void LerpToElement();
        protected abstract bool IndexShouldChangeFromDrag(PointerEventData data);

        protected abstract void SetContentSize(int elementCount);

    }
}