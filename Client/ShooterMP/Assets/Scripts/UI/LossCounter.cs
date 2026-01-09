using TMPro;
using UnityEngine;

namespace ShooterMP.UI
{
    public class LossCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private int _enemyLoss;
        private int _playerLoss;

        public void SetEnemyLoss(int value)
        {
            _enemyLoss = Mathf.Max(0, value);
            UpdateText();
        }

        public void SetPlayerLoss(int value)
        {
            _playerLoss = Mathf.Max(0, value);
            UpdateText();
        }

        private void UpdateText()
        {
            if (_text != null)
                _text.text = $"{_enemyLoss} : {_playerLoss}";
        }
    }
}
