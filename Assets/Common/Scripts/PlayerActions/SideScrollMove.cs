using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity2dCookbook
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SideScrollMove : MonoBehaviour
    {
        [SerializeField] private float _topSpeed = 4f;
        [SerializeField] private bool _instantTopSpeed = true;
        [SerializeField] private float _accelerationSpeed = .01f;
        [SerializeField] private float _deccelerationSpeed = .04f;
        
        private Rigidbody2D _rigidbody2D;
        private bool _moving;
        private float _moveVelocity;

        public bool IsMoving() { return _moving; }

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _moving = false;
            _moveVelocity = 0f;
        }

        private void Update()
        {
            Vector2 v = GameInput.Instance.GetSideScrollPlayerMoveVector(true);
            if (v.x > 0f || v.x < 0f)
            {
                _moving = true;
                if (_instantTopSpeed)
                {
                    _moveVelocity = v.x * _topSpeed;
                }
                else
                {
                    _moveVelocity = Mathf.Clamp(_moveVelocity + v.x * _accelerationSpeed * Time.deltaTime, -_topSpeed, _topSpeed);
                }
                _rigidbody2D.velocity = new Vector2(_moveVelocity, _rigidbody2D.velocity.y);
            }
            else
            {
                _moving = false;
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
                _rigidbody2D.velocity = new Vector2(_moveVelocity, _rigidbody2D.velocity.y);
            }
        }
    }
}