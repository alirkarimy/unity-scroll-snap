using Alec.Factory;
using Alec.Pool;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBulletPool", menuName = "Pool/Bullet Pool")]
public class BulletPoolSO : PoolSO<GameObject>
{
    [SerializeField]
    private BulletFactorySO _factory;

    public override IFactory<GameObject> Factory
    {
        get
        {
            return _factory;
        }
        set
        {
            _factory = value as BulletFactorySO;
        }
    }
    public override void Return(GameObject member)
    {
        member.SetActive(false);
        base.Return(member);
    }
    public override GameObject Request()
    {
        GameObject g = base.Request();
        g.SetActive(true);
        return g;
    }
}

