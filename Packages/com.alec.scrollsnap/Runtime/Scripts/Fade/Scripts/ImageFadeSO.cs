using UnityEngine;
using UnityEngine.UI;

namespace Alec.UI
{
    [CreateAssetMenu(fileName = "FadeImage", menuName = "Factory/Fade Image")]
    public class ImageFadeSO : ScriptableObject
    {

        [SerializeField] private ImageFadeColor[] fadeSOs;

        public void LerpFade(Image source, float lerpValue)
        {
            for (int i = 0; i < fadeSOs.Length; i++)
            {
                fadeSOs[i]?.Fade(source, lerpValue);
            }
        }

    }
}