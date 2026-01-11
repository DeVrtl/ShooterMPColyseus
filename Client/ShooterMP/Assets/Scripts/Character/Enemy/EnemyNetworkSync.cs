using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;

namespace ShooterMP.Character.Enemy
{
    using ShooterMP.Multiplayer;
    
    public class EnemyNetworkSync : MonoBehaviour
    {
        [SerializeField] private EnemyCharacter _character;

        private NetworkIntervalTracker _intervalTracker;
        private global::Player _player;

        public void Initialize(global::Player player)
        {
            _player = player;
            _intervalTracker = new NetworkIntervalTracker();
            
            _character.SetSpeed(player.speed);
            _character.SetMaxHP(player.maxHP);
            
            player.OnChange += OnPlayerStateChanged;
        }

        public void Cleanup()
        {
            _player.OnChange -= OnPlayerStateChanged;
        }

        private void OnPlayerStateChanged(List<DataChange> changes)
        {
            _intervalTracker.RecordReceiveTime();
            
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
                    case "isInvulnerable":
                        _character.SetInvulnerable((bool)dataChange.Value);
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
            
            _character.SetMovement(position, velocity, _intervalTracker.AverageInterval);
            _character.SetRotateX(headRotation.x);
            _character.SetRotateY(bodyRotation.y);
        }
    }
}

