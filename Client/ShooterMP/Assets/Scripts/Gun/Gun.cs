using System;
using UnityEngine;

namespace ShooterMP.Gun
{
    using ShooterMP.Bullet;
    
    public abstract class Gun : MonoBehaviour
    {
        [SerializeField] protected Bullet BulletPrefab;
        
        public event Action Shot;
        
        protected void InvokeShot()
        {
            Shot?.Invoke();
        }
    }
}
