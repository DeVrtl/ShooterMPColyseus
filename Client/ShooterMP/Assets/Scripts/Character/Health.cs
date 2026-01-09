using UnityEngine;
using ShooterMP.UI;

namespace ShooterMP.Character
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private HealthUI _ui;
        
        private int _max;
        private int _current;

        public int MaxHealth => _max;
        public int CurrentHealth => _current;

        public void SetMax(int max)
        {
            _max = Mathf.Max(1, max);
            _current = Mathf.Min(_current, _max);
            UpdateUI();
        }

        public void SetCurrent(int current)
        {
            _current = Mathf.Clamp(current, 0, _max);
            UpdateUI();
        }

        public void ApplyDamage(int damage)
        {
            if (damage < 0)
            {
                Debug.LogError($"Negative damage value: {damage}");
                return;
            }
            
            _current = Mathf.Max(0, _current - damage);
            UpdateUI();
        }

        public void Heal(int amount)
        {
            if (amount < 0)
            {
                Debug.LogError($"Negative heal amount: {amount}");
                return;
            }
            
            _current = Mathf.Min(_max, _current + amount);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_ui != null)
                _ui.UpdateHealth(_max, _current);
        }
    }
}
