using UnityEngine;

namespace ShooterMP.Character.Player
{
    using ShooterMP.Gun;
    
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter _playerCharacter;
        [SerializeField] private PlayerGun _gun;
        [SerializeField] private PlayerCursor _playerCursor;
        [SerializeField] private PlayerNetworkSender _networkSender;
        [SerializeField] private PlayerRespawnHandler _respawnHandler;
        [SerializeField] private float _mouseSensitivity = 5f;

        private void Update()
        {
            if (_respawnHandler.IsRespawning)
                return;
            
            ProcessInput();
            ProcessJumpInput();
            ProcessShootInput();
            
            SendMovementToServer();
        }

        private void ProcessInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            float mouseX = 0;
            float mouseY = 0;
            
            if (_playerCursor.IsCursorHidden)
            {
                mouseX = Input.GetAxis("Mouse X");
                mouseY = Input.GetAxis("Mouse Y");
            }
            
            _playerCharacter.SetupInput(horizontal, vertical, mouseX * _mouseSensitivity);
            _playerCharacter.RotateX(-mouseY * _mouseSensitivity);
        }

        private void ProcessJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _playerCharacter.Jump();
        }

        private void ProcessShootInput()
        {
            if (!_playerCursor.IsCursorHidden)
                return;
            
            bool isShoot = Input.GetMouseButton(0);
            
            if (isShoot && _gun.TryShoot(out ShootInfo shootInfo))
                _networkSender.SendShoot(ref shootInfo);
        }
        
        private void SendMovementToServer()
        {
            _playerCharacter.GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY);
            _networkSender.SendMove(position, velocity, rotateX, rotateY);
        }
    }
}
