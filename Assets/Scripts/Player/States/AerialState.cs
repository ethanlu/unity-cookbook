using System;
using Common.Events;
using Controls;
using Player.Animations;
using Player.Data;
using Utils;
using UnityEngine;

namespace Player.States
{
    public class AerialState : PlayerState
    {
        public AerialState(
            PlayerStateMachine stateMachine,
            Rigidbody2D physicsBody,
            BoxCollider2D physicsCollider,
            PlayerAnimator animator,
            PlayerHitBox hitBox,
            PlayerHurtBox hurtBox,
            MoveConfiguration moveConfiguration,
            JumpConfiguration jumpConfiguration,
            AttackConfiguration attackConfiguration
        ) : base(stateMachine, physicsBody, physicsCollider, animator, hitBox, hurtBox, moveConfiguration, jumpConfiguration, attackConfiguration)
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
                    _animator.AirAttacking(_attackSequence);
                    break;
                case 1: // combo attack
                    _attackSequence = 2;
                    _animator.AirAttacking(_attackSequence);
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
                    _animator.AirAttacking(_attackSequence);
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
            
            _animator.OnAnimationEvent += AnimationRecoveryEvent;
        }

        public override void Exit()
        {
            base.Exit();

            GameInput.Instance.OnStopAction -= StopAction;
            GameInput.Instance.OnMoveAction -= MoveAction;
            GameInput.Instance.OnJumpAction -= JumpAction;
            GameInput.Instance.OnAttackAction -= AttackAction;
            
            _animator.OnAnimationEvent -= AnimationRecoveryEvent;

            _airJumpCount = 0;
            _jumpVelocity = 0f;
        }

        public override void Update()
        {
            base.Update();
            
            if (_jumpVelocity > 0f)
            {
                _physicsBody.velocity += Vector2.up * _jumpVelocity;
                _jumpVelocity = 0f;
            }

            if (_physicsBody.velocity.y < 0f)
            {   // if vertical velocity is negative, then we must no longer be jumping and would be falling if we are not grounded
                _animator.Jumping(false);
                _animator.Falling(!_grounded);
            }

            if (_moveVelocity != 0f && _jumpConfiguration.AerialMove)
            {   // allow some movement while in the air
                ApplyMovementWithAcceleration();
            }

            if (_moveVelocity == 0f && _jumpConfiguration.AerialMove)
            {   // stopped moving but do a slight drift due to being in air
                ApplyStopWithDeceleration();
            }
            
            if (_grounded && _jumpVelocity == 0f && _physicsBody.velocity.y == 0f)
            {   // touching ground and there is no jump velocity to apply and current vertical velocity is 0....we are grounded
                _attackSequence = 0;
                _animator.AirAttacking(_attackSequence);
                _stateMachine.ChangeState(PlayerStateMachine.GroundedState);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}