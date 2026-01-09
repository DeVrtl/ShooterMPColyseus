using System;
using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;

public class PlayerCharacter : Character
{
    [SerializeField] private Health _health;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private CheckFly _checkFly;
    [SerializeField] private float _maxHeadAngle = 90f;
    [SerializeField] private float _minHeadAngle = -90f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _jumpDelay = 0.2f;
    
    private float _inputHorizontal;
    private float _inputVertical;
    private float _rotateY;
    private float _currentRotateX;
    private float _jumpTime;

    private void Start()
    {
        Transform camera = Camera.main.transform;
        
        camera.parent = _cameraPoint;
        camera.localPosition = Vector3.zero;
        camera.localRotation = Quaternion.identity;
        
        _health.SetMax(MaxHealth);
        _health.SetCurrent(MaxHealth);
    }

    private void FixedUpdate()
    {
        Move();
        RotateY();
    }

    public void SetupInput(float horizontal, float vertical, float rotateY)
    {
        _inputHorizontal = horizontal;
        _inputVertical = vertical;
        _rotateY += rotateY;
    }
    
    private void Move()
    {
        Vector3 velocity = (transform.forward * _inputVertical + transform.right * _inputHorizontal).normalized * Speed;
        velocity.y = _rigidbody.linearVelocity.y;
        Velocity = velocity;
        
        _rigidbody.linearVelocity = Velocity;
    }

    public void RotateX(float value)
    {
        _currentRotateX = Mathf.Clamp(_currentRotateX + value, _minHeadAngle, _maxHeadAngle);
        _head.localEulerAngles = new Vector3(_currentRotateX, 0, 0);
    }

    private void RotateY()
    {
        _rigidbody.angularVelocity = new Vector3(0, _rotateY, 0);
        _rotateY = 0;
    }
    
    public void GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY)
    {
        position = transform.position;
        velocity = _rigidbody.linearVelocity;
        
        rotateY = transform.rotation.eulerAngles.y;
        rotateX = _head.localEulerAngles.x;
    }

    public void Jump()
    {
        if (_checkFly.IsFly)
            return;
        
        if (Time.time - _jumpTime < _jumpDelay) 
            return;
        
        _jumpTime = Time.time;
        _rigidbody.AddForce(0, _jumpForce, 0, ForceMode.VelocityChange);
    }

    public void OnChange(List<DataChange> changes)
    {
        foreach (var dataChange in changes)
        {
            switch (dataChange.Field)
            {
                case "loss":
                    Multiplayer.Instance.LossCounter.SetPlayerLoss((byte)dataChange.Value);
                    break;
                case "currentHP":
                    _health.SetCurrent((sbyte)dataChange.Value);
                    break;
                
                default:
                    Debug.Log("Can't handle field: " + dataChange.Field);
                    break;
            }
        }
    }
}
