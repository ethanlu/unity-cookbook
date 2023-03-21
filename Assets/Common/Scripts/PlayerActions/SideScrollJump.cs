using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity2dCookbook
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class SideScrollJump : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _jumpSpeed = 10f;
        [SerializeField] private bool _airJump = true;
        [SerializeField] private int _maxAirJumps = 1;
        [SerializeField] private float _groundDistance = .5f;
        
        private Rigidbody2D _rigidbody2D;
        private BoxCollider2D _boxCollider2D;

        private int _airJumpCount;
        private bool _jumping;
        private bool _falling;

        private bool IsGrounded()
        {
            return Physics2D.BoxCast(_boxCollider2D.bounds.center, _boxCollider2D.bounds.size, 0, Vector2.down, _groundDistance, _groundLayer).collider is not null;
        }

        public bool IsJumping() { return _jumping; }
        public bool IsFalling() { return _falling; }

        void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _jumping = false;
            _falling = false;

            _airJumpCount = 0;
        }
        
        void Update()
        {
            var grounded = IsGrounded();
            if (_rigidbody2D.velocity.y < 0f)
            {
                _jumping = false;
                _falling = !grounded;
            }

            if (grounded)
            {
                _airJumpCount = 0;
            }

            if (Input.GetKeyDown("space") && (grounded || (_airJump && _jumping && _airJumpCount < _maxAirJumps)))
            {
                _airJumpCount += grounded ? 0 : 1;
                _jumping = true;
                _falling = false;
                _rigidbody2D.velocity += Vector2.up * _jumpSpeed;
            }
        }
    }
}