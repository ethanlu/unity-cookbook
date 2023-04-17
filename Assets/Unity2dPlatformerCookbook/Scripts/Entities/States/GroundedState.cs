using System;
using Unity2dPlatformerCookbook.Scripts.Controls;
using Unity2dPlatformerCookbook.Scripts.Utils;
using UnityEngine;

namespace Unity2dPlatformerCookbook.Scripts.Entities.States
{
    public class GroundedState : EntityState
    {
        public GroundedState(Entity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
        {
            GameInput.Instance.OnStopAction += StopAction;
            GameInput.Instance.OnMoveAction += MoveAction;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event delegates

        public void StopAction(object sender, EventArgs args)
        {
            _moveVelocity = 0f;
            _entity.EntityAnimator().Moving(false);
        }
        
        public void MoveAction(object sender, EventArgs args)
        {
            _moveVelocity = ((MoveEventArgs) args).Value.x * _entity.MoveConfiguration().TopSpeed;
            _facing = _moveVelocity > 0f ? Direction.Right : Direction.Left;
            _entity.EntityAnimator().Moving(true);
            _entity.EntityAnimator().Facing(_facing);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // interface methods

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
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