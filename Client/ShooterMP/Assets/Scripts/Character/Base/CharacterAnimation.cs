using UnityEngine;

namespace ShooterMP.Character.Base
{
    public class CharacterAnimation : MonoBehaviour
    {
        private const string GroundedParam = "Grounded";
        private const string SpeedParam = "Speed";
        
        [SerializeField] private Animator _animator;
        [SerializeField] private CheckFly _checkFly;
        [SerializeField] private Character _character;

        private void Update()
        {
            if (_character.Speed <= 0f)
                return;
                
            Vector3 localVelocity = _character.transform.InverseTransformVector(_character.Velocity);
            float speed = localVelocity.magnitude / _character.Speed;
            float sign = Mathf.Sign(localVelocity.z);
            
            _animator.SetFloat(SpeedParam, speed * sign);
            _animator.SetBool(GroundedParam, !_checkFly.IsFly);
        }
    }
}
