using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity2dCookbook
{
    public class MoveStateChangedEventArgs : EventArgs
    {
        public MoveState State { get; set; }
        public Direction Facing { get; set; }
    }
    
    [RequireComponent(typeof(Rigidbody2D))]
    public class SideScrollMove : MonoBehaviour
    {
        public event EventHandler<MoveStateChangedEventArgs> MoveStateChangedEvent;
        
        [SerializeField] private float _topSpeed = 4f;
        [SerializeField] private bool _instantTopSpeed = true;
        [SerializeField] private float _accelerationSpeed = .01f;
        [SerializeField] private float _deccelerationSpeed = .04f;

        private Rigidbody2D _rigidbody2D;
        private MoveState _moveState;
        private Direction _facing;
        private float _moveVelocity;

        private void UpdateState(MoveState state, Direction facing)
        {
            if (_moveState != state || _facing != facing)
            {   // notify subscribers only if state changed 
                MoveStateChangedEventArgs args = new MoveStateChangedEventArgs();
                args.State = state;
                args.Facing = facing;

                EventHandler<MoveStateChangedEventArgs> eventHandler = MoveStateChangedEvent;
                eventHandler?.Invoke(this, args);
            }
            _moveState = state;
            _facing = facing;
        }
        
        private void MoveAction(object sender, EventArgs args)
        {
            _moveVelocity = ((MoveEventArgs) args).Value.x * _topSpeed;
            UpdateState(MoveState.Moving, _moveVelocity > 0f ? Direction.Right : Direction.Left);
        }

        private void StopAction(object sender, EventArgs args)
        {
            _moveVelocity = 0f;
            UpdateState(MoveState.Idle, _facing);
        }

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _moveState = MoveState.Idle;
            _facing = Direction.Right;
            _moveVelocity = 0f;

            SideScrollGameInput.Instance.OnMoveAction += MoveAction;
            SideScrollGameInput.Instance.OnStopAction += StopAction;
        }
        
        private void Update()
        {
            if (_instantTopSpeed)
            {   // apply move velocity immediately
                _rigidbody2D.velocity = new Vector2(_moveVelocity, _rigidbody2D.velocity.y);
            }
            else
            {   // acclerate/decelerate to move velocity
                switch (_moveState)
                {
                    case MoveState.Moving:
                        _rigidbody2D.velocity = new Vector2(Mathf.Clamp(_rigidbody2D.velocity.x + (_moveVelocity > 0f ? 1f : -1f) * _accelerationSpeed * Time.deltaTime, -_topSpeed, _topSpeed), _rigidbody2D.velocity.y);
                        break;
                    case MoveState.Idle:
                        if (_rigidbody2D.velocity.x < 0f)
                        {
                            _rigidbody2D.velocity = new Vector2(Mathf.Clamp(_rigidbody2D.velocity.x + _deccelerationSpeed * Time.deltaTime, _rigidbody2D.velocity.x, _moveVelocity), _rigidbody2D.velocity.y);
                        }
                        if (_rigidbody2D.velocity.x > 0f)
                        {
                            _rigidbody2D.velocity = new Vector2(Mathf.Clamp(_rigidbody2D.velocity.x - _deccelerationSpeed * Time.deltaTime, _moveVelocity, _rigidbody2D.velocity.x), _rigidbody2D.velocity.y);
                        }
                        break;
                }
            }
        }
    }
}