using TMPro;
using UnityEngine;

namespace ElkaGames.UI
{
    public abstract class TextFadeColor : FadeSO<TextMeshProUGUI, Color>
    {
        public enum Limitations
        {
            R,
            G,
            B,
            A
        }

        public abstract override Color From { get; }
        public abstract override Color To { get; }
        public abstract override void Fade(TextMeshProUGUI source, float lerpValue);


    }
}