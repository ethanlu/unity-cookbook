using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity2dCookbook
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SideScrollMove : MonoBehaviour
    {
        [SerializeField] private float _topSpeed = 4f;
        [SerializeField] private bool _acceleration = false;
        
        private Rigidbody2D _rigidbody2D;
        private bool _moving;
        private float _currentSpeed;
        private float _accelerationSpeed;
        private float _deccelerationSpeed;

        public bool IsMoving() { return _moving; }

        void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _moving = false;
            _accelerationSpeed = _topSpeed / 100f;
            _deccelerationSpeed = _topSpeed / 50f;
            _currentSpeed = 0f;
        }
        
        void Update()
        {
            Vector2 v = GameInput.Instance.GetMovementVector(true);

            if (v.x > 0f || v.x < 0f)
            {
                _moving = true;
                if (_acceleration)
                {
                    _currentSpeed = Mathf.Clamp(_currentSpeed + v.x * _accelerationSpeed, -_topSpeed, _topSpeed);
                    _rigidbody2D.velocity = new Vector2(_currentSpeed, _rigidbody2D.velocity.y);
                }
                else
                {
                    _rigidbody2D.velocity = new Vector2(v.x * _topSpeed, _rigidbody2D.velocity.y);
                }
            }
            else
            {
                _moving = false;
                if (_acceleration)
                {
                    if (_currentSpeed < 0f)
                    {
                        _currentSpeed = Mathf.Clamp(_currentSpeed + _deccelerationSpeed, _currentSpeed, 0f);
                    }
                    if (_currentSpeed > 0f)
                    {
                        _currentSpeed = Mathf.Clamp(_currentSpeed - _deccelerationSpeed, 0f, _currentSpeed);
                    }
                    _rigidbody2D.velocity = new Vector2(_currentSpeed, _rigidbody2D.velocity.y);
                }
                else
                {
                    _rigidbody2D.velocity = new Vector2(0f, _rigidbody2D.velocity.y);
                }
            }
        }
    }
}