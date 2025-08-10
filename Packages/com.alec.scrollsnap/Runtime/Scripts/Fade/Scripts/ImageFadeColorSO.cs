using UnityEngine;
using UnityEngine.UI;

namespace Alec.UI
{
    [CreateAssetMenu(fileName = "FadeColor", menuName = "Factory/Fade Image Color")]
    public class ImageFadeColorSO : ImageFadeColor
    {
        public override Color From => _fadeOutColor;
        [SerializeField] private Color _fadeOutColor;
        public override Color To => _activeColor;
        [SerializeField] private Color _activeColor;

        protected Color activeItem;
        protected Color fadeOutItem;

        public override void Fade(Image source, float lerpValue)
        {
            fadeOutItem = From;
            activeItem = To;

            source.color = Color.Lerp(fadeOutItem, activeItem, lerpValue);
        }
    }
}