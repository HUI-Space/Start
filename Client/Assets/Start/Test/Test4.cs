using UnityEngine;

namespace Start.Test
{
    public class Test4 : MonoBehaviour
    {
        ObjectPoolManager _objectPoolManager;
        ObjectPool<TestPool> _pool;
        private  void Start()
        {
            _objectPoolManager = new ObjectPoolManager();
            _pool = _objectPoolManager.CreateObjectPool<TestPool>(10);
            for (int i = 0; i < 20; i++)
            {
                TestPool pool = _pool.Spawn();
                pool.Id = i;
                Debug.Log(pool.Id);
            }
        }
    }

    public class TestPool : IObject
    {
        public int Id;
    }
}