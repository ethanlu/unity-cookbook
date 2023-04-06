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

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _moveState = MoveState.Idle;
            _facing = Direction.Right;
            _moveVelocity = 0f;
        }

        private void Update()
        {
            Vector2 v = SideScrollGameInput.Instance.GetSideScrollPlayerMoveVector(true);
            if (v.x > 0f || v.x < 0f)
            {
                if (_instantTopSpeed)
                {
                    _moveVelocity = v.x * _topSpeed;
                }
                else
                {
                    _moveVelocity = Mathf.Clamp(_moveVelocity + v.x * _accelerationSpeed * Time.deltaTime, -_topSpeed, _topSpeed);
                }
                UpdateState(MoveState.Moving, v.x > 0f ? Direction.Right : Direction.Left);
            }
            else
            {
                if (_instantTopSpeed)
                {
                    _moveVelocity = 0f;
                }
                else
                {
                    if (_moveVelocity < 0f)
                    {
                        _moveVelocity = Mathf.Clamp(_moveVelocity + _deccelerationSpeed * Time.deltaTime, _moveVelocity, 0f);
                    }
                    if (_moveVelocity > 0f)
                    {
                        _moveVelocity = Mathf.Clamp(_moveVelocity - _deccelerationSpeed * Time.deltaTime, 0f, _moveVelocity);
                    }
                }
                UpdateState(MoveState.Idle, _facing);
            }
            _rigidbody2D.velocity = new Vector2(_moveVelocity, _rigidbody2D.velocity.y);
        }
    }
}