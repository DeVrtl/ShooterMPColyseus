using UnityEngine;

namespace ShooterMP.Character.Player
{
    using ShooterMP.Gun;
    
    public class PlayerGun : Gun
    {
        private const float SecondsPerMinute = 60f;
        
        [SerializeField] private Transform _bulletSpawnPoint;
        [SerializeField] private float _bulletSpeed = 20f;
        [SerializeField, Min(1f)] private int _fireRateRPM = 400;
        [SerializeField] private int _damage = 1;
        
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
            _shootDelay = SecondsPerMinute / _fireRateRPM;
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
            InvokeShot();
            
            info.pX = position.x;
            info.pY = position.y;
            info.pZ = position.z;
            
            info.dX = velocity.x;
            info.dY = velocity.y;
            info.dZ = velocity.z;
            
            return true;
        }
    }
}
