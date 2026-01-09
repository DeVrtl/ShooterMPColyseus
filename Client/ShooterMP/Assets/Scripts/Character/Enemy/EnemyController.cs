using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;

namespace ShooterMP.Character.Enemy
{
    using ShooterMP.Multiplayer;
    using ShooterMP.Gun;
    
    public class EnemyController : MonoBehaviour
    {
        private const int IntervalSamplesCount = 5;
        
        [SerializeField] private EnemyCharacter _character;
        [SerializeField] private EnemyGun _gun;

        private float _lastReceiveTime = 0f;
        private List<float> _receiveTimeIntervals = new (IntervalSamplesCount);
        private global::Player _player;
        private float _cachedAverageInterval = 0f;
        
        private float AverageInterval => _cachedAverageInterval;

        public void Initialize(string key, global::Player player)
        {
            _character.Initialize(key);
            
            _player = player;
            
            _character.SetSpeed(player.speed);
            _character.SetMaxHP(player.maxHP);
            
            for (int i = 0; i < IntervalSamplesCount; i++)
            {
                _receiveTimeIntervals.Add(0f);
            }
            
            player.OnChange += OnChanged;
        }

        public void Shoot(in ShootInfo info)
        {
            Vector3 position = new Vector3(info.pX, info.pY, info.pZ);
            Vector3 velocity = new Vector3(info.dX, info.dY, info.dZ);
            
            _gun.Shoot(position, velocity);
        }
        
        public void Destroy()
        {
            if (_player != null)
                _player.OnChange -= OnChanged;
        }
        
        private void OnChanged(List<DataChange> changes)
        {
            SaveReceiveTime();
            
            Vector3 position = _character.TargetPosition;
            Vector3 velocity = _character.Velocity;
            Vector3 headRotation = _character.TargetRotationHead.eulerAngles;
            Vector3 bodyRotation = _character.TargetRotationBody.eulerAngles;
            
            foreach (var dataChange in changes)
            {
                switch (dataChange.Field)
                {
                    case "loss":
                        Multiplayer.Instance.LossCounter.SetEnemyLoss((byte)dataChange.Value);
                        break;
                    case "currentHP":
                        if ((sbyte)dataChange.Value > (sbyte)dataChange.PreviousValue)
                            _character.RestoreHP((sbyte)dataChange.Value);
                        break;
                    case "pX":
                        position.x = (float)dataChange.Value;
                        break;
                    case "pY":
                        position.y = (float)dataChange.Value;
                        break;
                    case "pZ":
                        position.z = (float)dataChange.Value;
                        break;
                    
                    case "vX":
                        velocity.x = (float)dataChange.Value;
                        break;
                    case "vY":
                        velocity.y = (float)dataChange.Value;
                        break;
                    case "vZ":
                        velocity.z = (float)dataChange.Value;
                        break;
                    
                    case "rX":
                        headRotation.x = (float)dataChange.Value;
                        break;
                    case "rY":
                        bodyRotation.y = (float)dataChange.Value;
                        break;
                    
                    default:
                        Debug.LogWarning($"Can't handle field: {dataChange.Field}");
                        break;
                }
            }
            
            _character.SetMovement(position, velocity, AverageInterval);
            _character.SetRotateX(headRotation.x);
            _character.SetRotateY(bodyRotation.y);
        }

        private void SaveReceiveTime()
        {
            float interval = Time.time - _lastReceiveTime;
            _lastReceiveTime = Time.time;
            
            _receiveTimeIntervals.Add(interval);
            _receiveTimeIntervals.RemoveAt(0);
            
            float sum = 0f;
            for (int i = 0; i < _receiveTimeIntervals.Count; i++)
            {
                sum += _receiveTimeIntervals[i];
            }
            _cachedAverageInterval = sum / _receiveTimeIntervals.Count;
        }
    }
}
