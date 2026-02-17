using UnityEngine;
using UnityEngine.Pool;

public class GenericPool<T, TParam> where T : MonoBehaviour, IPoolable<TParam>
{
    private readonly ObjectPool<T> pool;

    public GenericPool(T prefab, Transform parent = null, int defaultCapacity = 10, int maxSize = 100)
    {
        pool = new ObjectPool<T>(
            createFunc: () =>
            {
                T instance = Object.Instantiate(prefab, parent);
                instance.gameObject.SetActive(false);
                return instance;
            },
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj =>
            {
                obj.OnDespawn();
                obj.gameObject.SetActive(false);
            },
            actionOnDestroy: obj => Object.Destroy(obj.gameObject),
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize);
    }

    public T Get(TParam param)
    {
        T obj = pool.Get();
        obj.OnSpawn(param);
        obj.SetPool(this);
        return obj;
    }

    public void Release(T obj)
    {
        pool.Release(obj);
    }
}
