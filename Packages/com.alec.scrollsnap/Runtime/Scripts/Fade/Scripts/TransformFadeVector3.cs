using UnityEngine;

namespace ElkaGames.UI
{
    public abstract class TransformFadeVector3 : FadeSO<Transform, Vector3>
    {
        public bool isRelative;

        public enum Limitations
        {
            X,
            Y,
            Z
        }

        public abstract override Vector3 From { get; }
        public abstract override Vector3 To { get; }
        public abstract override void Fade(Transform source, float lerpValue);


    }



}