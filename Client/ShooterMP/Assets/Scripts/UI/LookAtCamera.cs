using UnityEngine;

namespace ShooterMP.UI
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform _camera;

        private void Start()
        {
            _camera = Camera.main.transform;
        }

        private void Update()
        {
            if (_camera != null)
                transform.LookAt(_camera);
        }
    }
}
