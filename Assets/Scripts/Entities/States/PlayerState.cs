using System;
using Controls;
using Utils;
using UnityEngine;

namespace Entities.States
{
    public class PlayerState : IEntityState
    {
        protected Player _player;
        protected PlayerStateMachine _stateMachine;

        protected static bool _grounded;
        protected static Direction _facing;
        
        protected static float _moveVelocity;
        protected static float _pausedMoveVelocity;
        protected static float _jumpVelocity;
        protected static float _airJumpCount;
        
        protected static int _attackSequence;
        protected static bool _attackRecovery;

        public PlayerState(Player player, PlayerStateMachine stateMachine)
        {
            _player = player;
            _stateMachine = stateMachine;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event delegates
        
        public void StopAction(object sender, EventArgs args)
        {
            _moveVelocity = 0f;
            _pausedMoveVelocity = 0f;
            _player.PlayerAnimator().Moving(false);
        }
        
        public void MoveAction(object sender, EventArgs args)
        {
            if (_attackSequence == 0)
            {   // must be not attacking
                _moveVelocity = ((MoveEventArgs) args).Value.x * _player.MoveConfiguration().TopSpeed;
                _facing = _moveVelocity > 0f ? Direction.Right : Direction.Left;
                _player.PlayerAnimator().Moving(true);
                _player.PlayerAnimator().Facing(_facing);
            }
        }

        protected void JumpAction(object sender, EventArgs args)
        {
            if (_attackSequence == 0 && (_grounded || 
                (_player.JumpConfiguration().AirJump && 
                 _player.Rigidbody2D().velocity.y > -_player.JumpConfiguration().AirJumpWindow && 
                 _player.Rigidbody2D().velocity.y < _player.JumpConfiguration().AirJumpWindow && 
                 _airJumpCount < _player.JumpConfiguration().MaxAirJumps)))
            {   // only allow jump if we are grounded or double jump is enabled and we are around the apex of the jump and we still have air jump charges available
                _airJumpCount += _grounded ? 0 : 1;
                _jumpVelocity = _player.JumpConfiguration().JumpSpeed;
                
                _player.PlayerAnimator().Jumping(true);
                _player.PlayerAnimator().Falling(false);
                
                _stateMachine.ChangeState(PlayerStateMachine.AerialState);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // internal methods

        protected bool IsGrounded()
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                _player.BoxCollider2D().bounds.center, 
                _player.BoxCollider2D().bounds.size, 0, Vector2.down, 
                _player.JumpConfiguration().GroundDistance, 
                _player.JumpConfiguration().GroundLayer
            );

            // draw a debug line at the bottom of the object representing what the box raycast would be hitting
            Debug.DrawLine(
                new Vector3(_player.Rigidbody2D().position.x - _player.BoxCollider2D().bounds.extents.x, _player.Rigidbody2D().position.y - _player.JumpConfiguration().GroundDistance),
                new Vector3(_player.Rigidbody2D().position.x + _player.BoxCollider2D().bounds.extents.x, _player.Rigidbody2D().position.y - _player.JumpConfiguration().GroundDistance),
                hit.collider is null ? Color.green : Color.red
            );

            return hit.collider is not null;
        }

        protected void ApplyMovementInstantly()
        {
            _player.Rigidbody2D().velocity = new Vector2(_moveVelocity, _player.Rigidbody2D().velocity.y);
        }

        protected void ApplyMovementWithAcceleration()
        {
            _player.Rigidbody2D().velocity = new Vector2(
                Mathf.Clamp(
                    _player.Rigidbody2D().velocity.x + (_moveVelocity > 0f ? 1f : -1f) * _player.MoveConfiguration().AccelerationSpeed * Time.deltaTime,
                    -_player.MoveConfiguration().TopSpeed,
                    _player.MoveConfiguration().TopSpeed),
                _player.Rigidbody2D().velocity.y);
        }

        protected void ApplyStopInstantly()
        {
            _player.Rigidbody2D().velocity = new Vector2(0f, _player.Rigidbody2D().velocity.y);
        }

        protected void ApplyStopWithDeceleration()
        {
            if (_player.Rigidbody2D().velocity.x < 0f)
            {
                _player.Rigidbody2D().velocity = new Vector2(
                    Mathf.Clamp(
                        _player.Rigidbody2D().velocity.x + _player.MoveConfiguration().DecelerationSpeed * Time.deltaTime,
                        _player.Rigidbody2D().velocity.x,
                        _moveVelocity),
                    _player.Rigidbody2D().velocity.y);
            }

            if (_player.Rigidbody2D().velocity.x > 0f)
            {
                _player.Rigidbody2D().velocity = new Vector2(
                    Mathf.Clamp(
                        _player.Rigidbody2D().velocity.x - _player.MoveConfiguration().DecelerationSpeed * Time.deltaTime,
                        _moveVelocity,
                        _player.Rigidbody2D().velocity.x),
                    _player.Rigidbody2D().velocity.y);
            }
        }
        
        
        protected void InterruptMove()
        {
            if (_moveVelocity != 0f && _attackSequence > 0 && !_player.AttackConfiguration().AttackMove)
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