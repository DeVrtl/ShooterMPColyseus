using System.Collections.Generic;
using UnityEngine;

namespace ShooterMP.Multiplayer
{
    using ShooterMP.Character.Player;
    using ShooterMP.Character.Enemy;
    
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter _playerPrefab;
        [SerializeField] private Enemy _enemyPrefab;

        private Dictionary<string, Enemy> _enemies = new();

        public PlayerCharacter CreatePlayer(Player player)
        {
            Vector3 position = new Vector3(player.pX, player.pY, player.pZ);
            Quaternion rotation = Quaternion.Euler(0, player.rY, 0);
            
            PlayerCharacter playerCharacter = Instantiate(_playerPrefab, position, rotation);
            return playerCharacter;
        }

        public Enemy CreateEnemy(string key, Player player)
        {
            Vector3 position = new Vector3(player.pX, player.pY, player.pZ);
            
            Enemy enemy = Instantiate(_enemyPrefab, position, Quaternion.identity);
            enemy.Initialize(key, player);
            
            _enemies.Add(key, enemy);
            return enemy;
        }
        
        public void RemoveEnemy(string key)
        {
            if (!_enemies.ContainsKey(key))
                return;
            
            Enemy enemy = _enemies[key];
            enemy.Destroy();
            
            Destroy(enemy.gameObject);
            _enemies.Remove(key);
        }

        public Enemy GetEnemy(string key)
        {
            return _enemies.ContainsKey(key) ? _enemies[key] : null;
        }
    }
}

