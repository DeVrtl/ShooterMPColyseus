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

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.TryGetComponent(out EnemyCharacter enemy))
            {
                enemy.ApplyDamage(_damage);
            }
            
            DestroyBullet();
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
