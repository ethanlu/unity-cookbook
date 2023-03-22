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
            RaycastHit2D hit = Physics2D.BoxCast(_boxCollider2D.bounds.center, _boxCollider2D.bounds.size, 0, Vector2.down, _groundDistance, _groundLayer);
            
            // draw a debug line at the bottom of the object representing what the box raycast would be hitting
            Debug.DrawLine(
                new Vector3(_rigidbody2D.position.x - _boxCollider2D.bounds.extents.x, _rigidbody2D.position.y - _groundDistance), 
                new Vector3(_rigidbody2D.position.x  + _boxCollider2D.bounds.extents.x, _rigidbody2D.position.y - _groundDistance), 
                hit.collider is null ? Color.green : Color.red
            );

            return hit.collider is not null;
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
            {   // if vertical velocity is negative, then we must no longer be jumping and would be falling if we are not grounded
                _jumping = false;
                _falling = !grounded;
            }

            if (grounded)
            {
                _airJumpCount = 0;
            }

            if (Input.GetKeyDown("space") && (grounded || (_airJump && _rigidbody2D.velocity.y < 1f && _airJumpCount < _maxAirJumps)))
            {   // only allow jump if we are grounded or double jump is enabled and we are around the apex of the jump and we still have air jump charges available
                _airJumpCount += grounded ? 0 : 1;
                _jumping = true;
                _falling = false;
                _rigidbody2D.velocity += Vector2.up * _jumpSpeed;
            }
        }
    }
}