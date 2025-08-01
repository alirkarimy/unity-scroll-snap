using UnityEngine;
using UnityEngine.UI;

namespace ElkaGames.UI
{
    public abstract class ImageFadeColor : FadeSO<Image, Color>
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
        public abstract override void Fade(Image source, float lerpValue);


    }
}