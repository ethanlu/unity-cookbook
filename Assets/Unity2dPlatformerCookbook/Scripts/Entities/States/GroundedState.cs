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
                    _entity.Rigidbody2D().velocity = new Vector2(0f, _entity.Rigidbody2D().velocity.y);
                }
                else
                {   // decelerate to 0
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
            }
            else
            {   // moving
                if (_entity.MoveConfiguration().InstantTopSpeed)
                {   // immediately move
                    _entity.Rigidbody2D().velocity = new Vector2(_moveVelocity, _entity.Rigidbody2D().velocity.y);
                }
                else
                {   // acclerate to move velocity
                    _entity.Rigidbody2D().velocity = new Vector2(
                        Mathf.Clamp(
                            _entity.Rigidbody2D().velocity.x + (_moveVelocity > 0f ? 1f : -1f) * _entity.MoveConfiguration().AccelerationSpeed * Time.deltaTime,
                            -_entity.MoveConfiguration().TopSpeed,
                            _entity.MoveConfiguration().TopSpeed),
                        _entity.Rigidbody2D().velocity.y);
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