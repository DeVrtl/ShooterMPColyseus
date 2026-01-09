using UnityEngine;

namespace ShooterMP.Character.Enemy
{
    using ShooterMP.Gun;
    
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyCharacter _character;
        [SerializeField] private EnemyGun _gun;
        [SerializeField] private EnemyNetworkSync _networkSync;

        public void Initialize(string key, global::Player player)
        {
            _character.Initialize(key);
            _networkSync.Initialize(player);
        }

        public void Shoot(in ShootInfo info)
        {
            Vector3 position = new Vector3(info.pX, info.pY, info.pZ);
            Vector3 velocity = new Vector3(info.dX, info.dY, info.dZ);
            
            _gun.Shoot(position, velocity);
        }
        
        public void Destroy()
        {
            _networkSync.Cleanup();
        }
    }
}
