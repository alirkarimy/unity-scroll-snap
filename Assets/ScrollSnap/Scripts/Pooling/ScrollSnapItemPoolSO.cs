using UnityEngine;
using Alec.Factory;
using Alec.Pool;

namespace ElkaGames.UI.Scroll
{
    [CreateAssetMenu(fileName = "ScrollSnapItemPool", menuName = "Pool/Scroll Snap Item Pool")]
    public class ScrollSnapItemPoolSO : ComponentPoolSO<ScrollSnapItem>
    {
        [SerializeField]
        private ScrollSnapItemFactorySO _factory;
        public ScrollSnapItem Item => _factory?.loopItem;
        public override IFactory<ScrollSnapItem> Factory
        {
            get
            {
                return _factory;
            }
            set
            {
                _factory = value as ScrollSnapItemFactorySO;
            }
        }
        public override void Return(ScrollSnapItem member)
        {
            // member.SetActive(false);
            base.Return(member);
        }
        public override ScrollSnapItem Request()
        {
            ScrollSnapItem g = base.Request();
            // g.SetActive(true);
            return g;
        }
    }

}