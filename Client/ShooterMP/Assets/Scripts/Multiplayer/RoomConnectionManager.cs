using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colyseus;
using UnityEngine;

namespace ShooterMP.Multiplayer
{
    public class RoomConnectionManager
    {
        private ColyseusClient _client;
        private ColyseusRoom<State> _room;

        public ColyseusRoom<State> Room => _room;
        public string SessionId => _room?.SessionId;

        public async Task<ColyseusRoom<State>> ConnectToServerAsync(
            ColyseusClient client,
            Vector3 spawnPosition, 
            Vector3 spawnRotation,
            int spawnPointsCount,
            float playerSpeed,
            int playerMaxHealth)
        {
            _client = client;

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"points", spawnPointsCount},
                {"speed", playerSpeed},
                {"hp", playerMaxHealth},
                {"pX", spawnPosition.x},
                {"pY", spawnPosition.y},
                {"pZ", spawnPosition.z},
                {"rY", spawnRotation.y},
            };
            
            _room = await _client.JoinOrCreate<State>("state_handler", data);
            
            return _room;
        }

        public void SendMessage(string key, Dictionary<string, object> data)
        {
            _room?.Send(key, data);
        }
        
        public void SendMessage(string key, string data)
        {
            _room?.Send(key, data);
        }

        public void Disconnect()
        {
            _room?.Leave();
        }
    }
}

