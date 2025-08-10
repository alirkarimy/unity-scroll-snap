using UnityEngine;

namespace ElkaGames.UI
{
    public abstract class FadeSO<S, T> : ScriptableObject, IFade<S, T>
    {
        public abstract T From { get; }

        public abstract T To { get; }

        public abstract void Fade(S source, float lerpValue);
    }

    public interface IFade<out T>
    {
        T From { get; }
        T To { get; }
    }
    public interface IFade<S, out T> : IFade<T>
    {
        void Fade(S source, float lerpValue);
    }

    internal interface IFadeLimit<L>
    {
        void SetLimitations();
        L[] Limits { get; }
    }

}