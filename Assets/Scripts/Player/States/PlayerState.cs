using System;
using Common.FSM;
using Controls;
using Player.Animations;
using Player.Data;
using Utils;
using UnityEngine;

namespace Player.States
{
    public class PlayerState : IState
    {
        protected PlayerStateMachine _stateMachine;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // unity components

        protected Rigidbody2D _physicsBody;
        protected BoxCollider2D _physicsCollider;
        protected PlayerAnimator _animator;

        protected MoveConfiguration _moveConfiguration;
        protected JumpConfiguration _jumpConfiguration;
        protected AttackConfiguration _attackConfiguration;
        
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // state data

        protected static bool _grounded;
        protected static Direction _facing;
        
        protected static float _moveVelocity;
        protected static float _pausedMoveVelocity;
        protected static float _jumpVelocity;
        protected static float _airJumpCount;
        
        protected static int _attackSequence;
        protected static bool _attackRecovery;

        public PlayerState(
            PlayerStateMachine stateMachine,
            Rigidbody2D physicsBody,
            BoxCollider2D physicsCollider,
            PlayerAnimator animator,
            MoveConfiguration moveConfiguration,
            JumpConfiguration jumpConfiguration,
            AttackConfiguration attackConfiguration
        )
        {
            _stateMachine = stateMachine;
            _physicsBody = physicsBody;
            _physicsCollider = physicsCollider;
            _animator = animator;
            _moveConfiguration = moveConfiguration;
            _jumpConfiguration = jumpConfiguration;
            _attackConfiguration = attackConfiguration;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event delegates
        
        public void StopAction(object sender, EventArgs args)
        {
            _moveVelocity = 0f;
            _pausedMoveVelocity = 0f;
            _animator.Moving(false);
        }
        
        public void MoveAction(object sender, EventArgs args)
        {
            _moveVelocity = ((MoveEventArgs) args).Value.x * _moveConfiguration.TopSpeed;
            _facing = _moveVelocity > 0f ? Direction.Right : Direction.Left;
            _animator.Moving(true);
            _animator.Facing(_facing);
        }

        protected void JumpAction(object sender, EventArgs args)
        {
            if (_attackSequence == 0 && (_grounded || 
                (_jumpConfiguration.AirJump && 
                 _physicsBody.velocity.y > -_jumpConfiguration.AirJumpWindow && 
                 _physicsBody.velocity.y < _jumpConfiguration.AirJumpWindow && 
                 _airJumpCount < _jumpConfiguration.MaxAirJumps)))
            {   // only allow jump if we are grounded or double jump is enabled and we are around the apex of the jump and we still have air jump charges available
                _airJumpCount += _grounded ? 0 : 1;
                _jumpVelocity = _jumpConfiguration.JumpSpeed;
                
                _animator.Jumping(true);
                _animator.Falling(false);
                
                _stateMachine.ChangeState(PlayerStateMachine.AerialState);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // internal methods

        protected bool IsGrounded()
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                _physicsCollider.bounds.center, 
                _physicsCollider.bounds.size, 0, Vector2.down, 
                _jumpConfiguration.GroundDistance, 
                _jumpConfiguration.GroundLayer
            );

            // draw a debug line at the bottom of the object representing what the box raycast would be hitting
            Debug.DrawLine(
                new Vector3(_physicsBody.position.x - _physicsCollider.bounds.extents.x, _physicsBody.position.y - _jumpConfiguration.GroundDistance),
                new Vector3(_physicsBody.position.x + _physicsCollider.bounds.extents.x, _physicsBody.position.y - _jumpConfiguration.GroundDistance),
                hit.collider is null ? Color.green : Color.red
            );

            return hit.collider is not null;
        }

        protected void ApplyMovementInstantly()
        {
            _physicsBody.velocity = new Vector2(_moveVelocity, _physicsBody.velocity.y);
        }

        protected void ApplyMovementWithAcceleration()
        {
            _physicsBody.velocity = new Vector2(
                Mathf.Clamp(
                    _physicsBody.velocity.x + (_moveVelocity > 0f ? 1f : -1f) * _moveConfiguration.AccelerationSpeed * Time.deltaTime,
                    -_moveConfiguration.TopSpeed,
                    _moveConfiguration.TopSpeed),
                _physicsBody.velocity.y);
        }

        protected void ApplyStopInstantly()
        {
            _physicsBody.velocity = new Vector2(0f, _physicsBody.velocity.y);
        }

        protected void ApplyStopWithDeceleration()
        {
            if (_physicsBody.velocity.x < 0f)
            {
                _physicsBody.velocity = new Vector2(
                    Mathf.Clamp(
                        _physicsBody.velocity.x + _moveConfiguration.DecelerationSpeed * Time.deltaTime,
                        _physicsBody.velocity.x,
                        _moveVelocity),
                    _physicsBody.velocity.y);
            }

            if (_physicsBody.velocity.x > 0f)
            {
                _physicsBody.velocity = new Vector2(
                    Mathf.Clamp(
                        _physicsBody.velocity.x - _moveConfiguration.DecelerationSpeed * Time.deltaTime,
                        _moveVelocity,
                        _physicsBody.velocity.x),
                    _physicsBody.velocity.y);
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