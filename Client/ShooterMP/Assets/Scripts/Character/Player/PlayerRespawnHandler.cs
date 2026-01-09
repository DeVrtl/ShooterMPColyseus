using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShooterMP.Character.Player
{
    using ShooterMP.Multiplayer;
    
    public class PlayerRespawnHandler : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter _playerCharacter;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _restartDelay = 3f;

        private Multiplayer _multiplayer;
        private bool _isRespawning = false;
        
        public bool IsRespawning => _isRespawning;

        private void Start()
        {
            _multiplayer = Multiplayer.Instance;
        }

        public void OnRestart(int spawnIndex)
        {
            _multiplayer.SpawnPoints.GetPoint(spawnIndex, out Vector3 position, out Vector3 rotation);

            _canvasGroup.alpha = 1;
            
            StartCoroutine(RespawnCoroutine());

            _playerCharacter.transform.position = position;
            rotation.x = 0;
            rotation.z = 0;
            _playerCharacter.transform.eulerAngles = rotation;
            
            _playerCharacter.SetupInput(0, 0, 0);
            
            Dictionary<string, object> data = new()
            {
                {"pX", position.x},
                {"pY", position.y},
                {"pZ", position.z},
                {"vX", 0},
                {"vY", 0},
                {"vZ", 0},
                {"rX", 0},
                {"rY", rotation.y}
            };
            
            _multiplayer.SendMessage("move", data);
        }

        private IEnumerator RespawnCoroutine()
        {
            float elapsedTime = 0f;
            
            _isRespawning = true;
            
            while (elapsedTime < _restartDelay)
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, 0f, elapsedTime / _restartDelay);
                
                elapsedTime += Time.deltaTime;
                
                yield return null;
            }
            
            _isRespawning = false;
            _canvasGroup.alpha = 0f;
        }
    }
}

