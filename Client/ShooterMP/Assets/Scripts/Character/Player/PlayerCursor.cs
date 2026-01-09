using UnityEngine;

namespace ShooterMP.Character.Player
{
    public class PlayerCursor : MonoBehaviour
    {
        private bool _hideCursor = true;
        
        public bool IsCursorHidden => _hideCursor;

        private void Start()
        {
            SetCursorState(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleCursor();
            }
        }

        private void ToggleCursor()
        {
            _hideCursor = !_hideCursor;
            SetCursorState(_hideCursor);
        }

        private void SetCursorState(bool hidden)
        {
            _hideCursor = hidden;
            Cursor.lockState = _hideCursor ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}

