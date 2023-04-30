using System;
using Controls;
using Utils;
using UnityEngine;

namespace Entities.States
{
    public class EntityState : IEntityState
    {
        protected Entity _entity;
        protected EntityStateMachine _stateMachine;

        protected static bool _grounded;
        protected static Direction _facing;
        
        protected static float _moveVelocity;
        protected static float _pausedMoveVelocity;
        protected static float _jumpVelocity;
        protected static float _airJumpCount;
        
        protected static int _attackSequence;
        protected static bool _attackRecovery;

        public EntityState(Entity entity, EntityStateMachine stateMachine)
        {
            _entity = entity;
            _stateMachine = stateMachine;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event delegates
        
        public void StopAction(object sender, EventArgs args)
        {
            _moveVelocity = 0f;
            _pausedMoveVelocity = 0f;
            _entity.EntityAnimator().Moving(false);
        }
        
        public void MoveAction(object sender, EventArgs args)
        {
            _moveVelocity = ((MoveEventArgs) args).Value.x * _entity.MoveConfiguration().TopSpeed;
            _facing = _moveVelocity > 0f ? Direction.Right : Direction.Left;
            _entity.EntityAnimator().Moving(true);
            _entity.EntityAnimator().Facing(_facing);
        }

        protected void JumpAction(object sender, EventArgs args)
        {
            if (_attackSequence == 0 && (_grounded || 
                (_entity.JumpConfiguration().AirJump && 
                 _entity.Rigidbody2D().velocity.y > -_entity.JumpConfiguration().AirJumpWindow && 
                 _entity.Rigidbody2D().velocity.y < _entity.JumpConfiguration().AirJumpWindow && 
                 _airJumpCount < _entity.JumpConfiguration().MaxAirJumps)))
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
        // internal methods

        protected bool IsGrounded()
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                _entity.BoxCollider2D().bounds.center, 
                _entity.BoxCollider2D().bounds.size, 0, Vector2.down, 
                _entity.JumpConfiguration().GroundDistance, 
                _entity.JumpConfiguration().GroundLayer
            );

            // draw a debug line at the bottom of the object representing what the box raycast would be hitting
            Debug.DrawLine(
                new Vector3(_entity.Rigidbody2D().position.x - _entity.BoxCollider2D().bounds.extents.x, _entity.Rigidbody2D().position.y - _entity.JumpConfiguration().GroundDistance),
                new Vector3(_entity.Rigidbody2D().position.x + _entity.BoxCollider2D().bounds.extents.x, _entity.Rigidbody2D().position.y - _entity.JumpConfiguration().GroundDistance),
                hit.collider is null ? Color.green : Color.red
            );

            return hit.collider is not null;
        }

        protected void ApplyMovementInstantly()
        {
            _entity.Rigidbody2D().velocity = new Vector2(_moveVelocity, _entity.Rigidbody2D().velocity.y);
        }

        protected void ApplyMovementWithAcceleration()
        {
            _entity.Rigidbody2D().velocity = new Vector2(
                Mathf.Clamp(
                    _entity.Rigidbody2D().velocity.x + (_moveVelocity > 0f ? 1f : -1f) * _entity.MoveConfiguration().AccelerationSpeed * Time.deltaTime,
                    -_entity.MoveConfiguration().TopSpeed,
                    _entity.MoveConfiguration().TopSpeed),
                _entity.Rigidbody2D().velocity.y);
        }

        protected void ApplyStopInstantly()
        {
            _entity.Rigidbody2D().velocity = new Vector2(0f, _entity.Rigidbody2D().velocity.y);
        }

        protected void ApplyStopWithDeceleration()
        {
            if (_entity.Rigidbody2D().velocity.x < 0f)
            {
                _entity.Rigidbody2D().velocity = new Vector2(
                    Mathf.Clamp(
                        _entity.Rigidbody2D().velocity.x + _entity.MoveConfiguration().DecelerationSpeed * Time.deltaTime,
                        _entity.Rigidbody2D().velocity.x,
                        _moveVelocity),
                    _entity.Rigidbody2D().velocity.y);
            }

            if (_entity.Rigidbody2D().velocity.x > 0f)
            {
                _entity.Rigidbody2D().velocity = new Vector2(
                    Mathf.Clamp(
                        _entity.Rigidbody2D().velocity.x - _entity.MoveConfiguration().DecelerationSpeed * Time.deltaTime,
                        _moveVelocity,
                        _entity.Rigidbody2D().velocity.x),
                    _entity.Rigidbody2D().velocity.y);
            }
        }
        
        
        protected void InterruptMove()
        {
            if (_moveVelocity != 0f && _attackSequence > 0 && !_entity.AttackConfiguration().AttackMove)
            {   // if attack move is not enabled, stop move velocity
                _pausedMoveVelocity = _moveVelocity;
                _moveVelocity = 0f;
            }
        }

        protected void ResumeMove()
        {
            if (_pausedMoveVelocity != 0f)
            {
                _moveVelocity = _pausedMoveVelocity;
                _pausedMoveVelocity = 0f;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // interface methods

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void Update()
        {
            _grounded = IsGrounded();
        }

        public virtual void FixedUpdate()
        {
        }
    }
}