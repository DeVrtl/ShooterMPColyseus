using System;
using System.Collections;
using UnityEngine;

namespace ShooterMP.Bullet
{
    using ShooterMP.Character.Enemy;
    
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _lifeTime = 5f;
        
        private int _damage;
        
        public void Initialize(Vector3 velocity, int damage = 0)
        {
            _damage = Mathf.Max(0, damage);
            _rigidbody.linearVelocity = velocity;
            StartCoroutine(DelayedDestruction());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out EnemyCharacter enemy))
            {
                enemy.ApplyDamage(_damage);
            }
            
            DestroyBullet();
        }

        private void OnCollisionEnter(Collision other)
        {

        }

        private IEnumerator DelayedDestruction()
        {
            yield return new WaitForSecondsRealtime(_lifeTime);
            DestroyBullet();
        }
        
        private void DestroyBullet()
        {
            Destroy(gameObject);
        }
    }
}
