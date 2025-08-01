using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ElkaGames.UI.Scroll
{
    public class ScrollSnapAnimation : MonoBehaviour
    {

        [SerializeField] List<ScrollSnapItem> itemsList = new List<ScrollSnapItem>();
        public int ItemCount => itemsList.Count;
         private RectTransform _rect;
        [SerializeField] private RectTransform _centerObject;

        public Vector2 itemSize;
        public Vector2 ItemSize=> Utility.ConvertPixelToCanvasSize(GetComponentInParent<Canvas>(),itemSize.x,itemSize.y);
        private float distance;
        private float lerpValue;
        private ScrollSnapHorizontal.Layout layout = ScrollSnapHorizontal.Layout.Vertical;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }


        internal void Init(RectTransform viewport, ScrollSnapHorizontal.Layout layout = ScrollSnapHorizontal.Layout.Vertical)
        {
            this.layout = layout;
            Vector3 itemPos = Vector3.zero;
            switch (this.layout)
            {
                case ScrollSnapHorizontal.Layout.Horizontal:
                    _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemsList.Count * itemSize.x + viewport.rect.size.x / 2);
                    itemPos = new Vector3(_rect.rect.xMin + viewport.rect.size.x / 2, 0, 0);
                    break;
                case ScrollSnapHorizontal.Layout.Vertical:
                    _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemsList.Count * itemSize.y + viewport.rect.size.y / 2);
                    itemPos = new Vector3(0, _rect.rect.yMax - viewport.rect.size.y / 2 - itemSize.y / 2, 0);
                    break;
                case ScrollSnapHorizontal.Layout.Grid:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
           

            for (int i = 0; i < itemsList.Count; i++)
            {
                itemsList[i].Init(i.ToString());
                itemsList[i].mRect.anchoredPosition = itemPos;
                switch (this.layout)
                {
                    case ScrollSnapHorizontal.Layout.Horizontal:
                        itemPos.x += itemSize.x;
                        break;
                    case ScrollSnapHorizontal.Layout.Vertical:
                        itemPos.y -= itemSize.y;
                        break;
                    case ScrollSnapHorizontal.Layout.Grid:
                        throw new NotImplementedException();
                    default:
                        throw new NotImplementedException();
                }

            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                switch (layout)
                {
                    case ScrollSnapHorizontal.Layout.Horizontal:
                        distance = Mathf.Abs(_centerObject.position.x - itemsList[i].mRect.position.x);
                        lerpValue = itemSize.x * 0.6f / (distance);
                        itemsList[i].Adapt(lerpValue);
                        break;
                    case ScrollSnapHorizontal.Layout.Vertical:
                        distance = Mathf.Abs(_centerObject.position.y - itemsList[i].mRect.position.y);
                        lerpValue = itemSize.y * 0.6f / (distance);
                        itemsList[i].Adapt(lerpValue);
                        break;
                    case ScrollSnapHorizontal.Layout.Grid:
                        throw new NotImplementedException();
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public ScrollSnapItem GetItem(int index)
        {
            if (index > 0 && index < itemsList.Count)
            {
                return itemsList[index];
            }
            else
                return null;
        }
        public List<ScrollSnapItem> GetAllItems()
        {
            return itemsList;
        }
    }


}