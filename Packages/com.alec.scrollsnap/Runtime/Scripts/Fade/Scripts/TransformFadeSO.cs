using UnityEngine;

namespace Alec.UI
{
    [CreateAssetMenu(fileName = "FadeTransform", menuName = "Factory/Fade Transform")]
    public class TransformFadeSO : ScriptableObject
    {

        [SerializeField] public TransformFadeVector3[] fadeSOs;

        public void LerpFade(Transform source, float lerpValue)
        {
            for (int i = 0; i < fadeSOs.Length; i++)
            {
                fadeSOs[i]?.Fade(source, lerpValue);
            }
        }

    }



}