using System;
using Colyseus;
using UnityEngine;

namespace ShooterMP.Multiplayer
{
    using ShooterMP.Gun;
    
    public class NetworkMessageRouter
    {
        private Spawner _spawner;
        private string _sessionId;

        public void Initialize(ColyseusRoom<State> room, Spawner spawner, string sessionId)
        {
            _spawner = spawner;
            _sessionId = sessionId;
            
            room.OnMessage<string>("Shoot", OnShootReceived);
        }

        private void OnShootReceived(string jsonShootInfo)
        {
            ShootInfo shootInfo = JsonUtility.FromJson<ShootInfo>(jsonShootInfo);
            
            var enemy = _spawner.GetEnemy(shootInfo.key);
            
            if (enemy == null)
                throw new InvalidOperationException($"Received shoot from unknown enemy: {shootInfo.key}");
            
            enemy.Shoot(shootInfo);
        }
    }
}

