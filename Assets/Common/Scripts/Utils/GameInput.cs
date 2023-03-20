using Unity.VisualScripting;
using UnityEngine;

namespace Unity2dCookbook
{
    public sealed class GameInput
    {
        private static GameInput _instance = null;
        private InputActions _inputActions;

        private GameInput()
        {
            _inputActions = new InputActions();
            _inputActions.Enable();
        }

        public static GameInput Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameInput();
                }
                return _instance;
            }
        }

        public Vector2 GetMovementVector(bool normalize = false)
        {
            return normalize ? _inputActions.Player.Move.ReadValue<Vector2>().normalized : _inputActions.Player.Move.ReadValue<Vector2>();
        }
    }
}