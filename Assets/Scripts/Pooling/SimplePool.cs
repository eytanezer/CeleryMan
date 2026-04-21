using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public class SimplePool<T> : MonoSingleton<SimplePool<T>> where T: MonoBehaviour, IPoolable
    {
        [SerializeField] T prefab;
        [SerializeField] int initialSize = 10;
        [SerializeField] int increaseSize = 10;
    
        private readonly Stack<T> _available = new();

        private void Awake()
        {
            IncreasePool(initialSize);
        }

        public T Get()
        {
            if (_available.Count < 1)
            {
                IncreasePool(increaseSize);
            }
            var pooledObject = _available.Pop();
            pooledObject.gameObject.SetActive(true);

            pooledObject.Reset();
        
            return pooledObject;
        }
  
        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            _available.Push(obj);
        }
    
        private void IncreasePool(int size)
        {
            for (int i = 0; i < size; i++)
            {
                var instance = Instantiate(prefab, parent: this.transform);
                Return(instance);
            }
        }
    }
}