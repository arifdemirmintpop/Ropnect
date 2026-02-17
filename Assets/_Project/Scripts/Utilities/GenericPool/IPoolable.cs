public interface IPoolable<TParam>
{
    public void OnSpawn(TParam param);
    public void OnDespawn();

    public void SetPool(object pool); 
}
