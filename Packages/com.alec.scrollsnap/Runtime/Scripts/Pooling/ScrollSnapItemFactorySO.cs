using Alec.Factory;
using UnityEngine;

namespace ElkaGames.UI.Scroll
{
    [CreateAssetMenu(fileName = "ScrollSnapItem", menuName = "Factory/Scroll Snap Item Factory")]
    public class ScrollSnapItemFactorySO : FactorySO<ScrollSnapItem>
    {
        public ScrollSnapItem loopItem;
        public override ScrollSnapItem Create()
        {
            return Instantiate(loopItem);
        }
    }
}