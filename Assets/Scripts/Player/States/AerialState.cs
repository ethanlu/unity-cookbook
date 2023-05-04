using System;
using Player.Animations;
using Controls;
using Utils;
using UnityEngine;

namespace Player.States
{
    public class AerialState : PlayerState
    {
        public AerialState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
        {
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event delegates
        
        public void AttackAction(object sender, EventArgs args)
        {
            switch (_attackSequence)
            {
                case 0: // non-combo attack
                    _attackSequence = 2;
                    _player.PlayerAnimator().AirAttacking(_attackSequence);
                    break;
                case 1: // combo attack
                    _attackSequence = 2;
                    _player.PlayerAnimator().AirAttacking(_attackSequence);
                    break;
                default:
                    _attackSequence = 0;
                    break;
            }
        }
        
        private void AnimationRecoveryEvent(object sender, EventArgs args)
        {
            switch (((AnimationEventArgs) args).param1)
            {
                case "RecoveryEnd":
                    _attackSequence = 0;
                    _player.PlayerAnimator().AirAttacking(_attackSequence);
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // interface methods

        public override void Enter()
        {
            base.Enter();

            GameInput.Instance.OnStopAction += StopAction;
            GameInput.Instance.OnMoveAction += MoveAction;
            GameInput.Instance.OnJumpAction += JumpAction;
            GameInput.Instance.OnAttackAction += AttackAction;
            
            _player.PlayerAnimator().OnAnimationEvent += AnimationRecoveryEvent;
        }

        public override void Exit()
        {
            base.Exit();

            GameInput.Instance.OnStopAction -= StopAction;
            GameInput.Instance.OnMoveAction -= MoveAction;
            GameInput.Instance.OnJumpAction -= JumpAction;
            GameInput.Instance.OnAttackAction -= AttackAction;
            
            _player.PlayerAnimator().OnAnimationEvent -= AnimationRecoveryEvent;

            _airJumpCount = 0;
            _jumpVelocity = 0f;
        }

        public override void Update()
        {
            base.Update();
            
            if (_jumpVelocity > 0f)
            {
                _player.Rigidbody2D().velocity += Vector2.up * _jumpVelocity;
                _jumpVelocity = 0f;
            }

            if (_player.Rigidbody2D().velocity.y < 0f)
            {   // if vertical velocity is negative, then we must no longer be jumping and would be falling if we are not grounded
                _player.PlayerAnimator().Jumping(false);
                _player.PlayerAnimator().Falling(!_grounded);
            }

            if (_moveVelocity != 0f && _player.JumpConfiguration().AerialMove)
            {   // allow some movement while in the air
                ApplyMovementWithAcceleration();
            }

            if (_moveVelocity == 0f && _player.JumpConfiguration().AerialMove)
            {   // stopped moving but do a slight drift due to being in air
                ApplyStopWithDeceleration();
            }
            
            if (_grounded && _jumpVelocity == 0f && _player.Rigidbody2D().velocity.y == 0f)
            {   // touching ground and there is no jump velocity to apply and current vertical velocity is 0....we are grounded
                _attackSequence = 0;
                _player.PlayerAnimator().AirAttacking(_attackSequence);
                _stateMachine.ChangeState(PlayerStateMachine.GroundedState);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}