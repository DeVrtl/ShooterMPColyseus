using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private HealthUI _ui;
    
    private int _max;
    private int _current;

    public void SetMax(int max)
    {
        _max = max;
    }

    public void SetCurrent(int current)
    {
        _current = current;
        UpdateUI();
    }

    public void ApplyDamage(int damage)
    {
        _current -= damage;
        UpdateUI();
    }

    private void UpdateUI()
    {
        _ui.UpdateHealth(_max, _current);
    }
}
