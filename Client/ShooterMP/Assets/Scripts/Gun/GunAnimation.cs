using UnityEngine;

namespace ShooterMP.Gun
{
    public class GunAnimation : MonoBehaviour
    {
        private const string ShootTrigger = "Shoot";
        
        [SerializeField] private Gun _gun;
        [SerializeField] private Animator _animator;
        
        private void Start()
        {
            _gun.Shot += OnShot;
        }

        private void OnDestroy()
        {
            _gun.Shot -= OnShot;
        }

        private void OnShot()
        {
            _animator.SetTrigger(ShootTrigger);
        }
    }
}
