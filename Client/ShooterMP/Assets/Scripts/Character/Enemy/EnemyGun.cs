using UnityEngine;

public class EnemyGun : Gun
{
    public void Shoot(Vector3 position, Vector3 velocity)
    {
        Instantiate(BulletPrefab, position, Quaternion.identity).Initialize(velocity);
        
        Shot?.Invoke();
    }
}
