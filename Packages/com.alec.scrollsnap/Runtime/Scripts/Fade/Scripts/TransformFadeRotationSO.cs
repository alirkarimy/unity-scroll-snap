using System;
using UnityEngine;

namespace Alec.UI
{
    [CreateAssetMenu(fileName = "FadeRotation", menuName = "Factory/Fade Rotation")]
    public class TransformFadeRotationSO : TransformFadeVector3, IFadeLimit<TransformFadeVector3.Limitations>
    {
        public override Vector3 From => _fadeOutRotation;
        [SerializeField] protected Vector3 _fadeOutRotation;

        public override Vector3 To => _activeItemRotation;
        [SerializeField] protected Vector3 _activeItemRotation;

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
                _fadeOutRotation = transform.localEulerAngles;
                _activeItemRotation = _fadeOutRotation + _activeItemRotation;
            }
            fadeOutItem = From;
            activeItem = To;

            if (IsLimitationsSet == false)
                SetLimitations();

            ApplyLimitaions?.Invoke(transform);
            transform.localEulerAngles = Vector3.Lerp(fadeOutItem, activeItem, lerpValue);

        }


        public void SetLimitations()
        {
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