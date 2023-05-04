using System;
using Common.Events;
using Controls;
using Player.Animations;
using Player.Data;
using Utils;
using UnityEngine;

namespace Player.States
{
    public class GroundedState : PlayerState
    {
        public GroundedState(
            PlayerStateMachine stateMachine,
            Rigidbody2D physicsBody,
            BoxCollider2D physicsCollider,
            PlayerAnimator animator,
            MoveConfiguration moveConfiguration,
            JumpConfiguration jumpConfiguration,
            AttackConfiguration attackConfiguration
        ) : base(stateMachine, physicsBody, physicsCollider, animator, moveConfiguration, jumpConfiguration, attackConfiguration)
        {
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event delegates
        
        public void AttackAction(object sender, EventArgs args)
        {
            switch (_attackSequence)
            {
                case 0:     // initial attack if not in recovery
                    if (!_attackRecovery)
                    {
                        _attackSequence++;
                        _attackRecovery = false;
                        _animator.GroundAttacking(_attackSequence);
                    }
                    break;
                case 1:     // combo attack if we are in recovery phase
                    if (_attackRecovery)
                    {
                        _attackSequence++;
                        _attackRecovery = false;
                        _animator.GroundAttacking(_attackSequence);
                    }
                    break;
                case 2:     // combo attack if we are in recovery phase
                    if (_attackRecovery)
                    {
                        _attackSequence = 1;
                        _attackRecovery = false;

                        _jumpVelocity = _jumpConfiguration.JumpSpeed;
                        _animator.Jumping(true);

                        _animator.AirAttacking(_attackSequence);
                        _animator.GroundAttacking(0);

                        _stateMachine.ChangeState(PlayerStateMachine.AerialState);
                    }
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
                case "RecoveryStart":
                    _attackRecovery = true;
                    break;
                case "RecoveryEnd":
                    if (_attackRecovery)
                    {   // reached end of animation and a combo was not continued
                        _attackRecovery = false;
                        _attackSequence = 0;
                        _animator.GroundAttacking(_attackSequence);
                    }
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
        }

        public override void Update()
        {
            base.Update();
            
            if (_moveVelocity != 0f && _attackSequence > 0 && !_attackConfiguration.AttackMove)
            {   // there is a move velocity, but we are attacking and attack-move is not enabled....interrupt move
                _pausedMoveVelocity = _moveVelocity;
                _moveVelocity = 0f;
            }
            
            if (_attackSequence == 0 && _pausedMoveVelocity != 0f)
            {   // no longer attacking ...resume move velocity if there is any
                _moveVelocity = _pausedMoveVelocity;
                _pausedMoveVelocity = 0f;
            }

            if (_moveVelocity == 0f)
            {   // idling
                if (_moveConfiguration.InstantTopSpeed)
                {   // immediately stop
                    ApplyStopInstantly();
                }
                else
                {   // decelerate to 0
                    ApplyStopWithDeceleration();
                }
            }
            else
            {   // moving
                if (_moveConfiguration.InstantTopSpeed)
                {   // immediately move
                    ApplyMovementInstantly();
                }
                else
                {   // acclerate to move velocity
                    ApplyMovementWithAcceleration();
                }
            }
            
            if (!_grounded)
            {
                _stateMachine.ChangeState(PlayerStateMachine.AerialState);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}