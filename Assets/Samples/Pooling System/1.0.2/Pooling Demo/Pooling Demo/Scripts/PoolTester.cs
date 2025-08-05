using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolTester : MonoBehaviour
{
    [SerializeField]
    private BulletPoolSO _pool = default;
    int number = 2;
    int x = 20;
    private void Start()
    {
        List<GameObject> goList = _pool.Request(5) as List<GameObject>;

        foreach (GameObject go in goList)
        {
            _pool.Return(go);
        }
        StartCoroutine(RequestBullet(4));

    }

    private IEnumerator ReturnBullet(GameObject go)
    {
        yield return new WaitForSeconds(2f);
        _pool.Return(go);
    }
    private IEnumerator RequestBullet(int num = 1)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < num; i++)
            {
                GameObject go2 = _pool.Request() as GameObject;

                StartCoroutine(ReturnBullet(go2));
            }

        }



    }

}
