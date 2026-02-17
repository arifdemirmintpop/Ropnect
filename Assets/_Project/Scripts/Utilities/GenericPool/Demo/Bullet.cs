using UnityEngine;
using DG.Tweening;
public class Bullet : MonoBehaviour, IPoolable<Vector3>
{
    private GenericPool<Bullet, Vector3> pool;

    public void OnSpawn(Vector3 direction)
    {
       DOVirtual.DelayedCall(1,()=> pool.Release(this));
       
    }

    public void OnDespawn()
    {
    }

    public void SetPool(object pool)
    {
        this.pool = pool as GenericPool<Bullet, Vector3>;
    }

}
