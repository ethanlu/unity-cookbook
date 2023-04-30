using System;
using UnityEngine;

namespace Controls
{
    public class MoveEventArgs : EventArgs
    {
        public Vector2 Value { get; set; }
    }
    
    public sealed class GameInput
    {
        public event EventHandler OnMoveAction;
        public event EventHandler OnStopAction;
        public event EventHandler OnJumpAction;
        public event EventHandler OnAttackAction; 
        
        private static GameInput _instance = null;
        private InputActions _inputActions;

        private bool _normalizedMovements;

        private GameInput()
        {
            _inputActions = new InputActions();
            _inputActions.SideScrollPlayer.Enable();

            _normalizedMovements = true;

            _inputActions.SideScrollPlayer.Move.performed += MoveEvent;
            _inputActions.SideScrollPlayer.Move.canceled += StopEvent;
            _inputActions.SideScrollPlayer.Jump.performed += JumpEvent;
            _inputActions.SideScrollPlayer.Attack.performed += AttackEvent;
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

        private void AttackEvent(UnityEngine.InputSystem.InputAction.CallbackContext args)
        {
            OnAttackAction?.Invoke(this, EventArgs.Empty);
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

        public void NormalizedMovement(bool toggle) { _normalizedMovements = toggle; }
    }
}