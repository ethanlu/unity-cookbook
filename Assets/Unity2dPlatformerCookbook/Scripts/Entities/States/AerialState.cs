using System;
using Unity2dPlatformerCookbook.Scripts.Controls;
using Unity2dPlatformerCookbook.Scripts.Utils;
using UnityEngine;

namespace Unity2dPlatformerCookbook.Scripts.Entities.States
{
    public class AerialState : EntityState
    {
        public AerialState(Entity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
        {
            GameInput.Instance.OnJumpAction += JumpAction;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event delegates
        
        protected void JumpAction(object sender, EventArgs args)
        {
            if (_grounded || 
                (_entity.JumpConfiguration().AirJump && 
                 _entity.Rigidbody2D().velocity.y > -_entity.JumpConfiguration().AirJumpWindow && 
                 _entity.Rigidbody2D().velocity.y < _entity.JumpConfiguration().AirJumpWindow && 
                 _airJumpCount < _entity.JumpConfiguration().MaxAirJumps))
            {   // only allow jump if we are grounded or double jump is enabled and we are around the apex of the jump and we still have air jump charges available
                _airJumpCount += _grounded ? 0 : 1;
                _jumpVelocity = _entity.JumpConfiguration().JumpSpeed;
                
                _entity.EntityAnimator().Jumping(true);
                _entity.EntityAnimator().Falling(false);
                
                _stateMachine.ChangeState(EntityStateMachine.AerialState);
            }
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

            _airJumpCount = 0;
            _jumpVelocity = 0f;
        }

        public override void Update()
        {
            base.Update();
            
            if (_jumpVelocity > 0f)
            {
                _entity.Rigidbody2D().velocity += Vector2.up * _jumpVelocity;
                _jumpVelocity = 0f;
            }

            if (_entity.Rigidbody2D().velocity.y < 0f)
            {   // if vertical velocity is negative, then we must no longer be jumping and would be falling if we are not grounded
                _entity.EntityAnimator().Jumping(false);
                _entity.EntityAnimator().Falling(!_grounded);
            }

            if (_moveVelocity != 0f && _entity.JumpConfiguration().AerialMove)
            {   // allow some movement while in the air
                ApplyMovementWithAcceleration();
            }

            if (_moveVelocity == 0f)
            {   // stopped moving but do a slight drift due to being in air
                ApplyStopWithDeceleration();
            }

            if (_grounded)
            {
                _stateMachine.ChangeState(EntityStateMachine.GroundedState);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}