using System;
using UnityEngine;

public class PlayerGun : Gun
{
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private float _bulletSpeed = 20f;
    [SerializeField, Min(1f)] private int _fireRateRPM = 400;
    [SerializeField] private int _damage;
    
    private float _shootDelay;
    private float _lastShootTime;

    private void Start()
    {
        UpdateShootDelay();
    }
    
    private void OnValidate()
    {
        UpdateShootDelay();
    }

    private void UpdateShootDelay()
    {
        _shootDelay = 60f / _fireRateRPM;
        
        if (_fireRateRPM <= 0f)
            _shootDelay = 0f;
    }

    public bool TryShoot(out ShootInfo info)
    {
        info = new ShootInfo();
        
        if (Time.time - _lastShootTime < _shootDelay)
            return false;
        
        Vector3 position = _bulletSpawnPoint.position;
        Vector3 velocity = _bulletSpawnPoint.forward * _bulletSpeed;
        
        _lastShootTime = Time.time;
        Instantiate(BulletPrefab, position, Quaternion.identity).Initialize(velocity, _damage);
        Shot?.Invoke();
        
        info.pX = position.x;
        info.pY = position.y;
        info.pZ = position.z;
        
        info.dX = velocity.x;
        info.dY = velocity.y;
        info.dZ = velocity.z;
        
        return true;
    }
}
