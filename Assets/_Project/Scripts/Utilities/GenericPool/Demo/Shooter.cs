using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    private GenericPool<Bullet, Vector3> bulletPool;

    private void Start()
    {
        bulletPool = new GenericPool<Bullet, Vector3>(bulletPrefab, transform, 10, 50);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 shootDir = transform.forward;
            Bullet bullet = bulletPool.Get(shootDir);
            bullet.transform.position = transform.position;
        }
    }
}
