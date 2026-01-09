using System;
using UnityEngine;

public class CheckFly : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _radius;
    [SerializeField] private float _coyoteTime = 0.2f;

    private float _flyTimer = 0f;
    
    public bool IsFly { get; private set; } = true;

    private void Update()
    {
        if (Physics.CheckSphere(transform.position, _radius, _layerMask))
        {
            IsFly = false;
            _flyTimer = 0f;
        }
        else
        {
            _flyTimer += Time.deltaTime;

            if (_flyTimer >= _coyoteTime)
                IsFly = true;
            
        }
    }
}
