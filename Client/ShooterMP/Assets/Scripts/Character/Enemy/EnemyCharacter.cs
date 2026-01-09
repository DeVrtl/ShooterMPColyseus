using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] private Health _health;
    [SerializeField] private Transform _head;
    
    public Vector3 TargetPosition { get; private set; } = Vector3.zero;
    public Quaternion TargetRotationHead { get; private set; } = Quaternion.identity;
    public Quaternion TargetRotationBody { get; private set; } = Quaternion.identity;
    
    private string _sessionID;
    private float _velocityMagnitude = 0;
    
    //private float _bodyRotateSpeed = 1000f;
    //private float _headRotateSpeed = 1000f;
    
    private void Start()
    {
        TargetPosition = transform.position;

        TargetRotationBody = transform.localRotation;
        TargetRotationHead = _head.localRotation;
    }

    private void Update()
    {
        if (_velocityMagnitude > .1f)
        {
            float maxDistance = _velocityMagnitude * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, maxDistance);
        }
        else
        {
            transform.position = TargetPosition;
        }

        float angleBody = Quaternion.Angle(transform.rotation, TargetRotationBody);
        transform.rotation = angleBody > 110f ? TargetRotationBody : Quaternion.Slerp(transform.rotation, TargetRotationBody, 45f * Time.deltaTime);
        
        float angleHead = Quaternion.Angle(_head.localRotation, TargetRotationHead);
        _head.localRotation = angleHead > 45 ? TargetRotationHead : Quaternion.Slerp(_head.localRotation, TargetRotationHead, 45f * Time.deltaTime);   
        
        /*
         Или
         
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            TargetRotationBody,
            _bodyRotateSpeed * Time.deltaTime
        );

        // Плавный поворот головы к целевому
        _head.localRotation = Quaternion.RotateTowards(
            _head.localRotation,
            TargetRotationHead,
            _headRotateSpeed * Time.deltaTime
        );
        */
    }

    public void Initialize(string sessionID)
    {
        _sessionID = sessionID;
    }
    
	public void ApplyDamage(int damage)
	{
        _health.ApplyDamage(damage);

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"id", _sessionID},
            {"value", damage}
        };
        
        Multiplayer.Instance.SendMessage("damage", data);
    }

    public void SetSpeed(float value) => Speed = value;

    public void SetMaxHP(int value)
    {
        MaxHealth = value;
        
        _health.SetMax(value);
        _health.SetCurrent(value);
    }

    public void RestoreHP(int newValue)
    {
        _health.SetCurrent(newValue);
    }
    
    public void SetMovement(in Vector3 position, in Vector3 velocity, in float averageInterval)
    {
        TargetPosition = position + (velocity * averageInterval);
        
        _velocityMagnitude = velocity.magnitude;
        
        Velocity = velocity;
    }

    public void SetRotateX(float value)
    {
        TargetRotationHead = Quaternion.Euler(value, 0f, 0f);
    }

    public void SetRotateY(float value)
    {
        TargetRotationBody = Quaternion.Euler(0f, value, 0f);
    }
}
