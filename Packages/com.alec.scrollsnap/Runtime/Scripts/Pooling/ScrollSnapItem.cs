using UnityEngine;
using UnityEngine.UI;
namespace Alec.UI.Scroll
{
    public class ScrollSnapItem : MonoBehaviour
    {
        public string data;
        [SerializeField] private TransformFadeSO fadeSO;
        [SerializeField] private ImageFadeSO imagefadeSO;
        [SerializeField] private float sizeMultiplier = 1;

        internal RectTransform _mRect;
        internal RectTransform mRect { get { if (!_mRect) _mRect = GetComponent<RectTransform>(); return _mRect; } }

        public Vector2 Size => mRect.sizeDelta * transform.localScale.sqrMagnitude * sizeMultiplier;

        internal void Init(string data)
        {
            this.data = data;
        }

        /// <summary>
        /// this method adapt list item according to its distance from center of loop list
        /// </summary>
        /// <param name="lerpValue">is a flaot number between 0,1</param>
        internal void Adapt(float lerpValue)
        {

            fadeSO?.LerpFade(transform, lerpValue);
            imagefadeSO?.LerpFade(GetComponent<Image>(), lerpValue);
        }

    }
}