using System.Collections.Generic;
using UnityEngine;

namespace ShooterMP.Character.Enemy
{
    public class EnemyCharacter : Base.Character
    {
        private const float DefaultBodyRotationSpeed = 45f;
        private const float DefaultHeadRotationSpeed = 45f;
        private const float BodyRotationThreshold = 110f;
        private const float HeadRotationThreshold = 45f;
        private const float MinVelocityThreshold = 0.1f;
        
        [SerializeField] private Health _health;
        [SerializeField] private Transform _head;
        
        public Vector3 TargetPosition { get; private set; } = Vector3.zero;
        public Quaternion TargetRotationHead { get; private set; } = Quaternion.identity;
        public Quaternion TargetRotationBody { get; private set; } = Quaternion.identity;
        
        private string _sessionID;
        private float _velocityMagnitude = 0f;
        private bool _isInvulnerable = false;
        
        public bool IsInvulnerable => _isInvulnerable;
        
        private void Start()
        {
            TargetPosition = transform.position;
            TargetRotationBody = transform.localRotation;
            TargetRotationHead = _head.localRotation;
        }

        private void Update()
        {
            UpdatePosition();
            UpdateRotations();
        }

        private void UpdatePosition()
        {
            if (_velocityMagnitude > MinVelocityThreshold)
            {
                float maxDistance = _velocityMagnitude * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, TargetPosition, maxDistance);
            }
            else
            {
                transform.position = TargetPosition;
            }
        }

        private void UpdateRotations()
        {
            float angleBody = Quaternion.Angle(transform.rotation, TargetRotationBody);
            transform.rotation = angleBody > BodyRotationThreshold 
                ? TargetRotationBody 
                : Quaternion.Slerp(transform.rotation, TargetRotationBody, DefaultBodyRotationSpeed * Time.deltaTime);
            
            float angleHead = Quaternion.Angle(_head.localRotation, TargetRotationHead);
            _head.localRotation = angleHead > HeadRotationThreshold 
                ? TargetRotationHead 
                : Quaternion.Slerp(_head.localRotation, TargetRotationHead, DefaultHeadRotationSpeed * Time.deltaTime);
        }

        public void Initialize(string sessionID)
        {
            _sessionID = sessionID;
        }
        
        public void ApplyDamage(int damage)
        {
            if (damage < 0)
                return;
            
            if (_isInvulnerable)
                return;
            
            _health.ApplyDamage(damage);

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"id", _sessionID},
                {"value", damage}
            };
            
            Multiplayer.Multiplayer.Instance.SendMessage("damage", data);
        }

        public void SetSpeed(float value) => Speed = Mathf.Max(0f, value);

        public void SetMaxHP(int value)
        {
            MaxHealth = Mathf.Max(1, value);
            
            _health.SetMax(MaxHealth);
            _health.SetCurrent(MaxHealth);
        }

        public void RestoreHP(int newValue)
        {
            _health.SetCurrent(newValue);
        }
        
        public void SetMovement(in Vector3 position, in Vector3 velocity, in float averageInterval)
        {
            TargetPosition = position + (velocity * averageInterval);
            _velocityMagnitude = velocity.magnitude;
            Velocity = velocity;
        }

        public void SetRotateX(float value)
        {
            TargetRotationHead = Quaternion.Euler(value, 0f, 0f);
        }

        public void SetRotateY(float value)
        {
            TargetRotationBody = Quaternion.Euler(0f, value, 0f);
        }
        
        public void SetInvulnerable(bool isInvulnerable)
        {
            _isInvulnerable = isInvulnerable;
        }
    }
}
