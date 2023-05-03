using System;
using Player.Animations;
using Controls;
using Utils;
using UnityEngine;

namespace Player.States
{
    public class GroundedState : PlayerState
    {
        public GroundedState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
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
                        _player.PlayerAnimator().GroundAttacking(_attackSequence);
                    }
                    break;
                case 1:     // combo attack if we are in recovery phase
                    if (_attackRecovery)
                    {
                        _attackSequence++;
                        _attackRecovery = false;
                        _player.PlayerAnimator().GroundAttacking(_attackSequence);
                    }
                    break;
                case 2:     // combo attack if we are in recovery phase
                    if (_attackRecovery)
                    {
                        _attackSequence = 1;
                        _attackRecovery = false;

                        _jumpVelocity = _player.JumpConfiguration().JumpSpeed;
                        _player.PlayerAnimator().Jumping(true);

                        _player.PlayerAnimator().AirAttacking(_attackSequence);
                        _player.PlayerAnimator().GroundAttacking(0);

                        _stateMachine.ChangeState(PlayerStateMachine.AerialState);
                    }
                    break;
                default:
                    _attackSequence = 0;
                    break;
            }

            InterruptMove();
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
                        _player.PlayerAnimator().GroundAttacking(_attackSequence);

                        ResumeMove();
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
            
            _player.PlayerAnimator().OnAnimationEvent += AnimationRecoveryEvent;
            ResumeMove();
        }

        public override void Exit()
        {
            base.Exit();

            GameInput.Instance.OnStopAction -= StopAction;
            GameInput.Instance.OnMoveAction -= MoveAction;
            GameInput.Instance.OnJumpAction -= JumpAction;
            GameInput.Instance.OnAttackAction -= AttackAction;
            
            _player.PlayerAnimator().OnAnimationEvent -= AnimationRecoveryEvent;
        }

        public override void Update()
        {
            base.Update();

            if (_moveVelocity == 0f)
            {   // idling
                if (_player.MoveConfiguration().InstantTopSpeed)
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
                if (_player.MoveConfiguration().InstantTopSpeed)
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