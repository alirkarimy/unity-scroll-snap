using System;
using UnityEngine;

namespace ElkaGames.UI
{
    [CreateAssetMenu(fileName = "FadeScale", menuName = "Factory/Fade Scale")]
    public class TransformFadeScaleSO : TransformFadeVector3, IFadeLimit<TransformFadeVector3.Limitations>
    {
        public override Vector3 From => _fadeOutScale;
        [SerializeField] protected Vector3 _activeItemScale;

        public override Vector3 To => _activeItemScale;
        [SerializeField] protected Vector3 _fadeOutScale;

        protected Vector3 activeItem;
        protected Vector3 fadeOutItem;
        public TransformFadeVector3.Limitations[] Limits => limitations;
        [SerializeField] private TransformFadeVector3.Limitations[] limitations;

        [NonSerialized] private bool IsLimitationsSet = false;
        private event Action<Transform> ApplyLimitaions;


        public override void Fade(Transform transform, float lerpValue)
        {
            if (IsLimitationsSet == false && isRelative == true)
            {
                _fadeOutScale = transform.localScale.MultiplyElements(_fadeOutScale);
                _activeItemScale = transform.localScale.MultiplyElements(_activeItemScale);
            }
            fadeOutItem = From;
            activeItem = To;

            if (IsLimitationsSet == false)
                SetLimitations();

            ApplyLimitaions?.Invoke(transform);
            transform.localScale = Vector3.Lerp(fadeOutItem, activeItem, lerpValue);

        }

        public void SetFromTo(Vector3 from, Vector3 to)
        {
            _activeItemScale = from;
            _fadeOutScale = to;
        }

        public void SetLimitations()
        {
            if (Limits == null) return;
            for (int i = 0; i < Limits.Length; i++)
            {
                switch (Limits[i])
                {
                    case Limitations.X:
                        ApplyLimitaions += (transform) =>
                        {
                            fadeOutItem.x = transform.localEulerAngles.x;
                            activeItem.x = fadeOutItem.x;
                        };
                        break;
                    case Limitations.Y:
                        ApplyLimitaions += (transform) =>
                        {
                            fadeOutItem.y = transform.localEulerAngles.y;
                            activeItem.y = fadeOutItem.y;
                        };
                        break;
                    case Limitations.Z:
                        ApplyLimitaions += (transform) =>
                        {
                            fadeOutItem.z = transform.localEulerAngles.z;
                            activeItem.z = fadeOutItem.z;
                        };
                        break;
                }
            }
            IsLimitationsSet = true;
        }
    }

}