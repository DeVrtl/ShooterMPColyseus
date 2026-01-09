using UnityEngine;

namespace ShooterMP.UI
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _filledImage;
        [SerializeField] private float _defaultWidth;
        
        private void OnValidate()
        {
            if (_filledImage != null)
                _defaultWidth = _filledImage.sizeDelta.x;
        }

        public void UpdateHealth(int max, int current)
        {
            if (_filledImage == null)
            {
                Debug.LogWarning("Filled image is not assigned!");
                return;
            }
            
            if (max <= 0)
            {
                Debug.LogWarning($"Invalid max health value: {max}");
                return;
            }
            
            float percent = Mathf.Clamp01((float)current / max);
            _filledImage.sizeDelta = new Vector2(_defaultWidth * percent, _filledImage.sizeDelta.y);
        }
    }
}
