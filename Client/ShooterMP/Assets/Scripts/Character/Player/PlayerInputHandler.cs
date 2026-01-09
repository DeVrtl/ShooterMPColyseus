using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _playerCharacter;
    [SerializeField] private PlayerGun _gun;
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private float _restartDelay = 3f;

    private Multiplayer _multiplayer;
    private bool _hold = false;
    private bool _hideCursor = true;
    
    private void Start()
    {
        _multiplayer = Multiplayer.Instance;
        
        _hideCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _hideCursor = !_hideCursor;
            Cursor.lockState = _hideCursor ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        if (_hold)
            return;
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float mouseX = 0;
        float mouseY = 0;
        bool isShoot = false;
        
        if (_hideCursor)
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            isShoot = Input.GetMouseButton(0);
        }
        
        bool space = Input.GetKeyDown(KeyCode.Space);
        
        _playerCharacter.SetupInput(horizontal, vertical, mouseX * _mouseSensitivity);
        _playerCharacter.RotateX(-mouseY * _mouseSensitivity);

        if (space)
            _playerCharacter.Jump();

        if (isShoot && _gun.TryShoot(out ShootInfo shootInfo))
            SendShoot(ref shootInfo);
        
        SendMove();
    }

    private void SendShoot(ref ShootInfo shootInfo)
    {
        shootInfo.key = _multiplayer.GetSessionID();
        
        string json = JsonUtility.ToJson(shootInfo);
        
        _multiplayer.SendMessage("shoot", json);
    }
    
    private void SendMove()
    {
        _playerCharacter.GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY);
        Dictionary<string, object> data = new()
        {
            {"pX", position.x},
            {"pY", position.y},
            {"pZ", position.z},
            {"vX", velocity.x},
            {"vY", velocity.y},
            {"vZ", velocity.z},
            {"rX", rotateX},
            {"rY", rotateY}
        };
        _multiplayer.SendMessage("move", data);
    }

    public void OnRestart(int spawnIndex)
    {
        _multiplayer.SpawnPoints.GetPoint(spawnIndex, out Vector3 position, out Vector3 rotation);

        StartCoroutine(Hold());

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

    private IEnumerator Hold()
    {
        _hold = true;
        yield return new WaitForSecondsRealtime(_restartDelay);
        _hold = false;
    }
}
