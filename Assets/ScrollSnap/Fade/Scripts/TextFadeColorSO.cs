using TMPro;
using UnityEngine;

namespace ElkaGames.UI
{
    [CreateAssetMenu(fileName = "FadeColor", menuName = "Factory/Fade Text Color")]
    public class TextFadeColorSO : TextFadeColor
    {
        public override Color From => _fadeOutColor;
        [SerializeField] private Color _fadeOutColor;
        public override Color To => _activeColor;
        [SerializeField] private Color _activeColor;

        protected Color activeItem;
        protected Color fadeOutItem;

        public override void Fade(TextMeshProUGUI source, float lerpValue)
        {
            fadeOutItem = From;
            activeItem = To;

            source.color = Color.Lerp(fadeOutItem, activeItem, lerpValue);
        }
    }
}