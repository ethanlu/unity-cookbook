using System;
using Animations;
using Controls;
using Utils;
using UnityEngine;

namespace Entities.States
{
    public class GroundedState : EntityState
    {
        public GroundedState(Entity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
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
                        _entity.EntityAnimator().GroundAttacking(_attackSequence);
                    }
                    break;
                case 1:     // combo attack if we are in recovery phase
                    if (_attackRecovery)
                    {
                        _attackSequence++;
                        _attackRecovery = false;
                        _entity.EntityAnimator().GroundAttacking(_attackSequence);
                    }
                    break;
                case 2:     // combo attack if we are in recovery phase
                    if (_attackRecovery)
                    {
                        _attackSequence = 1;
                        _attackRecovery = false;

                        _jumpVelocity = _entity.JumpConfiguration().JumpSpeed;
                        _entity.EntityAnimator().Jumping(true);

                        _entity.EntityAnimator().AirAttacking(_attackSequence);
                        _entity.EntityAnimator().GroundAttacking(0);

                        _stateMachine.ChangeState(EntityStateMachine.AerialState);
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
                    _attackRecovery = false;
                    _attackSequence = 0;
                    _entity.EntityAnimator().GroundAttacking(_attackSequence);

                    ResumeMove();
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
            
            _entity.EntityAnimator().OnAnimationEvent += AnimationRecoveryEvent;
            ResumeMove();
        }

        public override void Exit()
        {
            base.Exit();

            GameInput.Instance.OnStopAction -= StopAction;
            GameInput.Instance.OnMoveAction -= MoveAction;
            GameInput.Instance.OnJumpAction -= JumpAction;
            GameInput.Instance.OnAttackAction -= AttackAction;
            
            _entity.EntityAnimator().OnAnimationEvent -= AnimationRecoveryEvent;
        }

        public override void Update()
        {
            base.Update();

            if (_moveVelocity == 0f)
            {   // idling
                if (_entity.MoveConfiguration().InstantTopSpeed)
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
                if (_entity.MoveConfiguration().InstantTopSpeed)
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
                _stateMachine.ChangeState(EntityStateMachine.AerialState);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}