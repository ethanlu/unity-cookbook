using System;
using UnityEngine;

namespace Unity2dCookbook
{
    public class MoveEventArgs : EventArgs
    {
        public Vector2 Value { get; set; }
    }
    
    public sealed class SideScrollGameInput
    {
        public event EventHandler OnMoveAction;
        public event EventHandler OnStopAction;
        public event EventHandler OnJumpAction;
        
        private static SideScrollGameInput _instance = null;
        private InputActions _inputActions;

        private bool _normalizedMovements;

        private SideScrollGameInput()
        {
            _inputActions = new InputActions();
            _inputActions.SideScrollPlayer.Enable();

            _normalizedMovements = true;

            _inputActions.SideScrollPlayer.Move.performed += MoveEvent;
            _inputActions.SideScrollPlayer.Move.canceled += StopEvent;
            _inputActions.SideScrollPlayer.Jump.performed += JumpEvent;
        }

        private void MoveEvent(UnityEngine.InputSystem.InputAction.CallbackContext args)
        {
            if (OnMoveAction is not null)
            {
                MoveEventArgs a = new MoveEventArgs();
                a.Value = _normalizedMovements ? args.ReadValue<Vector2>().normalized : args.ReadValue<Vector2>();
                OnMoveAction(this, a);
            }
        }
        
        private void StopEvent(UnityEngine.InputSystem.InputAction.CallbackContext args)
        {
            OnStopAction?.Invoke(this, EventArgs.Empty);
        }

        private void JumpEvent(UnityEngine.InputSystem.InputAction.CallbackContext args)
        {
            OnJumpAction?.Invoke(this, EventArgs.Empty);
        }

        public static SideScrollGameInput Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = new SideScrollGameInput();
                }
                return _instance;
            }
        }

        public void NormalizedMovement(bool toggle) { _normalizedMovements = toggle; }
    }
}