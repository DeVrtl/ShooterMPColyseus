using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colyseus;
using ShooterMP.Character.Player;
using ShooterMP.UI;
using UnityEngine;

namespace ShooterMP.Multiplayer
{
    public class Multiplayer : ColyseusManager<Multiplayer>
    {
        [field: SerializeField] public LossCounter LossCounter { get; private set; }
        [field: SerializeField] public SpawnPoints SpawnPoints { get; private set; }
        
        [SerializeField] private PlayerCharacter _playerPrefab;
        [SerializeField] private Spawner _spawner;

        private RoomConnectionManager _connectionManager;
        private NetworkMessageRouter _messageRouter;
        private ColyseusRoom<State> _room;
        
        protected override void Awake()
        {
            base.Awake();
            
            _connectionManager = new RoomConnectionManager();
            _messageRouter = new NetworkMessageRouter();
            
            Instance.InitializeClient();
            ConnectAsync();
        }

        private async void ConnectAsync()
        {
            try
            {
                await ConnectToServerAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to connect to server: {ex.Message}");
            }
        }

        private async Task ConnectToServerAsync()
        {
            SpawnPoints.GetPoint(UnityEngine.Random.Range(0, SpawnPoints.Length), out Vector3 spawnPosition, out Vector3 spawnRotation);
            
            _room = await _connectionManager.ConnectToServerAsync(
                Instance.client,
                spawnPosition,
                spawnRotation,
                SpawnPoints.Length,
                _playerPrefab.Speed,
                _playerPrefab.MaxHealth
            );
            
            _room.OnStateChange += OnStateChanged;
            _room.OnError += OnRoomError;
            
            _messageRouter.Initialize(_room, _spawner, _connectionManager.SessionId);
        }

        private void OnRoomError(int code, string message)
        {
            Debug.LogError($"Room error [{code}]: {message}");
        }

        public void SendMessage(string key, Dictionary<string, object> data)
        {
            _connectionManager.SendMessage(key, data);
        }
        
        public void SendMessage(string key, string data)
        {
            _connectionManager.SendMessage(key, data);
        }

        public string GetSessionID() => _connectionManager.SessionId;
        
        private void OnStateChanged(State state, bool isFirstState)
        {
            if (!isFirstState)
                return;

            state.players.ForEach(((key, player) =>
            {
                if (key == _room.SessionId)
                    CreatePlayer(player);
                else
                    _spawner.CreateEnemy(key, player);
            }));

            _room.State.players.OnAdd += (key, player) => _spawner.CreateEnemy(key, player);
            _room.State.players.OnRemove += (key, value) => _spawner.RemoveEnemy(key);
        }

        private void CreatePlayer(Player player)
        {
            PlayerCharacter playerCharacter = _spawner.CreatePlayer(player);
            player.OnChange += playerCharacter.OnChange;
            
            var respawnHandler = playerCharacter.GetComponent<PlayerRespawnHandler>();
            if (respawnHandler != null)
            {
                _room.OnMessage<int>("Restart", respawnHandler.OnRestart);
            }
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_room != null)
            {
                _room.OnError -= OnRoomError;
            }
            
            _connectionManager?.Disconnect();
        }
    }
}
