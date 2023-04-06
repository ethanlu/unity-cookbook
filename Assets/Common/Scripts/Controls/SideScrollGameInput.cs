using System;
using UnityEngine;

namespace Unity2dCookbook
{
    public sealed class SideScrollGameInput
    {
        public event EventHandler OnJumpAction;
        
        private static SideScrollGameInput _instance = null;
        private InputActions _inputActions;

        private SideScrollGameInput()
        {
            _inputActions = new InputActions();
            _inputActions.SideScrollPlayer.Enable();

            _inputActions.SideScrollPlayer.Jump.performed += JumpEvent;
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

        public Vector2 GetSideScrollPlayerMoveVector(bool normalize = false)
        {
            return normalize ? _inputActions.SideScrollPlayer.Move.ReadValue<Vector2>().normalized : _inputActions.SideScrollPlayer.Move.ReadValue<Vector2>();
        }

        public bool GetSideScrollPlayerJumpValue()
        {
            //return _inputActions.SideScrollPlayer.Jump
            return false;
        }
    }
}