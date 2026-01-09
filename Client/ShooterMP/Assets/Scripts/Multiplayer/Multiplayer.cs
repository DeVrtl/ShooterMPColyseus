using System;
using System.Collections.Generic;
using Colyseus;
using UnityEngine;
using Random = System.Random;

public class Multiplayer : ColyseusManager<Multiplayer>
{
    [field: SerializeField] public LossCounter LossCounter { get; private set; }
    [field: SerializeField] public SpawnPoints SpawnPoints { get; private set; }
    
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private EnemyController _enemy;

    private Dictionary<string, EnemyController> _enemies = new Dictionary<string, EnemyController>();
    private ColyseusRoom<State> _room;
    
    protected override void Awake()
    {
        base.Awake();
        
        Instance.InitializeClient();
        Connect();
    }

    private async void Connect()
    {
        SpawnPoints.GetPoint(UnityEngine.Random.Range(0, SpawnPoints.Lenght), out Vector3 spawnPosition, out Vector3 spawnRotation);
        
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"points", SpawnPoints.Lenght},
            {"speed", _player.Speed},
            {"hp", _player.MaxHealth},
            {"pX", spawnPosition.x},
            {"pY", spawnPosition.y},
            {"pZ", spawnPosition.z},
            {"rY", spawnRotation.y},
        };
        
       _room = await Instance.client.JoinOrCreate<State>("state_handler", data);
       
       _room.OnStateChange += OnStateChanged;

       _room.OnMessage<string>("Shoot", ApplyShoot);
       
    }

    public void SendMessage(string key, Dictionary<string, object> data)
    {
        _room.Send(key, data);
    }
    
    public void SendMessage(string key, string data)
    {
        _room.Send(key, data);
    }

    public string GetSessionID() => _room.SessionId;
    
    private void OnStateChanged(State state, bool isFirstState)
    {
        if (isFirstState == false)
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
        if (_enemies.ContainsKey(key) == false)
            return;
        
        EnemyController enemy = _enemies[key];
        enemy.Destroy();
        
        _enemies.Remove(key);
    }
    
    private void ApplyShoot(string jsonShootInfo)
    {
        ShootInfo shootInfo = JsonUtility.FromJson<ShootInfo>(jsonShootInfo);
        
        if (_enemies.ContainsKey(shootInfo.key) == false)
            throw new InvalidOperationException("There's no enemy, but he tried to shoot");
        
        _enemies[shootInfo.key].Shoot(shootInfo);
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();

        _room.Leave();
    }
}
