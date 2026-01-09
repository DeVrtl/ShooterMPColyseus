using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colyseus;
using ShooterMP.Character.Enemy;
using ShooterMP.Character.Player;
using ShooterMP.Gun;
using ShooterMP.UI;
using UnityEngine;

namespace ShooterMP.Multiplayer
{
    public class Multiplayer : ColyseusManager<Multiplayer>
    {
        [field: SerializeField] public LossCounter LossCounter { get; private set; }
        [field: SerializeField] public SpawnPoints SpawnPoints { get; private set; }
        
        [SerializeField] private PlayerCharacter _player;
        [SerializeField] private EnemyController _enemy;

        private Dictionary<string, EnemyController> _enemies = new ();
        private ColyseusRoom<State> _room;
        
        protected override void Awake()
        {
            base.Awake();
            
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
            
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"points", SpawnPoints.Length},
                {"speed", _player.Speed},
                {"hp", _player.MaxHealth},
                {"pX", spawnPosition.x},
                {"pY", spawnPosition.y},
                {"pZ", spawnPosition.z},
                {"rY", spawnRotation.y},
            };
            
            _room = await Instance.client.JoinOrCreate<State>("state_handler", data);
            
            _room.OnStateChange += OnStateChanged;
            _room.OnError += OnRoomError;
            _room.OnMessage<string>("Shoot", ApplyShoot);
        }

        private void OnRoomError(int code, string message)
        {
            throw new Exception($"Room error [{code}]: {message}");
        }

        public void SendMessage(string key, Dictionary<string, object> data)
        {
            _room.Send(key, data);
        }
        
        public void SendMessage(string key, string data)
        {
            _room.Send(key, data);
        }

        public string GetSessionID() => _room?.SessionId;
        
        private void OnStateChanged(State state, bool isFirstState)
        {
            if (!isFirstState)
                return;

            state.players.ForEach(((key, player) =>
            {
                if (key == _room.SessionId)
                    CreatePlayer(player);
                else
                    CreateEnemy(key, player);
            }));

            _room.State.players.OnAdd += CreateEnemy;
            _room.State.players.OnRemove += RemoveEnemy;
        }

        private void CreatePlayer(Player player)
        {
            Vector3 position = new Vector3(player.pX, player.pY, player.pZ);
            Quaternion rotation = Quaternion.Euler(0, player.rY, 0);
            
            PlayerCharacter playerCharacter = Instantiate(_player, position, rotation);
            player.OnChange += playerCharacter.OnChange;
            
            _room.OnMessage<int>("Restart", playerCharacter.GetComponent<PlayerInputHandler>().OnRestart);
        }

        private void CreateEnemy(string key, Player player)
        {
            Vector3 position = new Vector3(player.pX, player.pY, player.pZ);
            
            EnemyController enemy = Instantiate(_enemy, position, Quaternion.identity);
            enemy.Initialize(key, player);
            
            _enemies.Add(key, enemy);
        }
        
        private void RemoveEnemy(string key, Player value)
        {
            if (!_enemies.ContainsKey(key))
                return;
            
            EnemyController enemy = _enemies[key];
            enemy.Destroy();
            
            _enemies.Remove(key);
        }
        
        private void ApplyShoot(string jsonShootInfo)
        {
            ShootInfo shootInfo = JsonUtility.FromJson<ShootInfo>(jsonShootInfo);
            
            if (!_enemies.ContainsKey(shootInfo.key))
                throw new InvalidOperationException($"Received shoot from unknown enemy: {shootInfo.key}");
            
            _enemies[shootInfo.key].Shoot(shootInfo);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_room != null)
            {
                _room.OnError -= OnRoomError;
                _room.Leave();
            }
        }
    }
}
