using System;
using UnityEngine;

namespace Alec.UI
{
    [CreateAssetMenu(fileName = "FadePosition", menuName = "Factory/Fade Position")]
    public class TransformFadePositionSO : TransformFadeVector3, IFadeLimit<TransformFadeVector3.Limitations>
    {
        public override Vector3 To => _activeItemPosition;
        [SerializeField] private Vector3 _activeItemPosition;
        public override Vector3 From => _fadeOutPosition;
        [SerializeField] private Vector3 _fadeOutPosition;
        private Vector3 activeItem;

        private Vector3 fadeOutItem;
        public Limitations[] Limits => limitations;
        [SerializeField] private Limitations[] limitations;

        [NonSerialized] private bool IsLimitationsSet = false;
        private event Action<Transform> ApplyLimitaions;


        public override void Fade(Transform transform, float lerpValue)
        {
            if (IsLimitationsSet == false && isRelative == true)
            {
                _fadeOutPosition = transform.localPosition;
                _activeItemPosition = _fadeOutPosition + _activeItemPosition;
            }

            fadeOutItem = From;
            activeItem = To;

            if (IsLimitationsSet == false)
                SetLimitations();

            ApplyLimitaions?.Invoke(transform);
            transform.localPosition = Vector3.Lerp(fadeOutItem, activeItem, lerpValue);

        }
        public void SetFromTo(Vector3 from, Vector3 to)
        {
            _activeItemPosition = from;
            _fadeOutPosition = to;
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
                            fadeOutItem.x = transform.localPosition.x;
                            activeItem.x = fadeOutItem.x;
                        };
                        break;
                    case Limitations.Y:
                        ApplyLimitaions += (transform) =>
                        {
                            fadeOutItem.y = transform.localPosition.y;
                            activeItem.y = fadeOutItem.y;
                        };
                        break;
                    case Limitations.Z:
                        ApplyLimitaions += (transform) =>
                        {
                            fadeOutItem.z = transform.localPosition.z;
                            activeItem.z = fadeOutItem.z;
                        };
                        break;
                }
            }
            IsLimitationsSet = true;
        }
    }

}