using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Helper
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private Queue<T> _pool;
        private T _prefab;
        private Transform _parent;

        public ObjectPool(T prefab, int initialSize, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            _pool = new Queue<T>();

            for (int i = 0; i < initialSize; i++)
            {
                T obj = GameObject.Instantiate(_prefab, _parent);
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            while (_pool.Count > 0)
            {
                T obj = _pool.Dequeue();
                if (obj != null && !obj.Equals(null))
                {
                    obj.gameObject.SetActive(true);
                    return obj;
                }
            }
            T newObj = GameObject.Instantiate(_prefab, _parent);
            newObj.gameObject.SetActive(true);
            return newObj;
        }

        public void Release(T obj)
        {
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}