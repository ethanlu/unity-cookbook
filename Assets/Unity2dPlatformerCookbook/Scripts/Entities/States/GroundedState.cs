using System;
using Unity2dPlatformerCookbook.Scripts.Animations;
using Unity2dPlatformerCookbook.Scripts.Controls;
using Unity2dPlatformerCookbook.Scripts.Utils;
using UnityEngine;

namespace Unity2dPlatformerCookbook.Scripts.Entities.States
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
                        _entity.EntityAnimator().Attack(_attackSequence);
                    }
                    break;
                case 1:     // combo attack if we are in recovery phase
                    if (_attackRecovery)
                    {
                        _attackSequence++;
                        _entity.EntityAnimator().Attack(_attackSequence);
                    }
                    break;
                default:
                    _attackSequence = 0;
                    break;
            }
        }
        
        private void AnimationRecoveryEvent(object sender, EventArgs args)
        {
            switch (((AnimationEventArgs) args).name)
            {
                case "RecoveryStart":
                    _attackRecovery = true;
                    break;
                case "RecoveryEnd":
                    _attackRecovery = false;
                    _attackSequence = 0;
                    _entity.EntityAnimator().Attack(_attackSequence);
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