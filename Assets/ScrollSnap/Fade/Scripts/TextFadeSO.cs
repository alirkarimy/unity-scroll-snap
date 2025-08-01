using TMPro;
using UnityEngine;

namespace ElkaGames.UI
{
    [CreateAssetMenu(fileName = "FadeText", menuName = "Factory/Fade Text")]
    public class TextFadeSO : ScriptableObject
    {

        [SerializeField] private TextFadeColor[] fadeSOs;

        public void LerpFade(TextMeshProUGUI source, float lerpValue)
        {
            for (int i = 0; i < fadeSOs.Length; i++)
            {
                fadeSOs[i]?.Fade(source, lerpValue);
            }
        }

    }
}