using System;
using UnityEngine;

namespace Unity2dCookbook
{
    [Serializable]
    public class MoveConfiguration
    {
        public MoveConfiguration()
        {
            InstantTopSpeed = true;
            TopSpeed = 4f;
            AccelerationSpeed = 10f;
            DecelerationSpeed = 15f;
        }

        public bool InstantTopSpeed;
        public float TopSpeed;
        public float AccelerationSpeed;
        public float DecelerationSpeed;
    }
    
    [Serializable]
    public class JumpConfiguration
    {
        public JumpConfiguration()
        {
            JumpSpeed = 10f;
            AirJump = true;
            MaxAirJumps = 1;
            AirJumpWindow = 2f;
            GroundDistance = .5f;
        }

        public LayerMask GroundLayer;
        public float JumpSpeed;
        public bool AirJump;
        public int MaxAirJumps;
        public float AirJumpWindow;
        public float GroundDistance;
    }

    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SideScrollAnimation))]
    public class SideScrollEntity : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // customizable fields
        
        [SerializeField] protected GameObject _visual;
        [SerializeField] protected MoveConfiguration _moveConfiguration;
        [SerializeField] protected JumpConfiguration _jumpConfiguration;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // unity components
        
        protected BoxCollider2D _boxCollider2D;
        protected Rigidbody2D _rigidbody2D;
        
        protected SideScrollAnimation _sideScrollAnimation;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // internal properties

        protected bool _grounded;
        
        // states
        protected Direction _facing;
        protected MoveState _moveState;
        protected JumpState _jumpState;

        // move properties
        protected float _moveVelocity;
        
        // jump properties
        protected int _airJumpCount;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // internal methods
        
        protected bool IsGrounded()
        {
            RaycastHit2D hit = Physics2D.BoxCast(_boxCollider2D.bounds.center, _boxCollider2D.bounds.size, 0, Vector2.down, _jumpConfiguration.GroundDistance, _jumpConfiguration.GroundLayer);
            
            // draw a debug line at the bottom of the object representing what the box raycast would be hitting
            Debug.DrawLine(
                new Vector3(_rigidbody2D.position.x - _boxCollider2D.bounds.extents.x, _rigidbody2D.position.y - _jumpConfiguration.GroundDistance), 
                new Vector3(_rigidbody2D.position.x  + _boxCollider2D.bounds.extents.x, _rigidbody2D.position.y - _jumpConfiguration.GroundDistance), 
                hit.collider is null ? Color.green : Color.red
            );

            return hit.collider is not null;
        }

        private void ApplyMove()
        {
            if (_moveConfiguration.InstantTopSpeed)
            {   // apply move velocity immediately
                _rigidbody2D.velocity = new Vector2(_moveVelocity, _rigidbody2D.velocity.y);
            }
            else
            {   // acclerate/decelerate to move velocity
                switch (_moveState)
                {
                    case MoveState.Moving:
                        _rigidbody2D.velocity = new Vector2(
                            Mathf.Clamp(
                                _rigidbody2D.velocity.x + (_moveVelocity > 0f ? 1f : -1f) * _moveConfiguration.AccelerationSpeed * Time.deltaTime, 
                                -_moveConfiguration.TopSpeed, 
                                _moveConfiguration.TopSpeed), 
                            _rigidbody2D.velocity.y);
                        break;
                    case MoveState.Idle:
                        if (_rigidbody2D.velocity.x < 0f)
                        {
                            _rigidbody2D.velocity = new Vector2(
                                Mathf.Clamp(
                                    _rigidbody2D.velocity.x + _moveConfiguration.DecelerationSpeed * Time.deltaTime, 
                                    _rigidbody2D.velocity.x, 
                                    _moveVelocity), 
                                _rigidbody2D.velocity.y);
                        }
                        if (_rigidbody2D.velocity.x > 0f)
                        {
                            _rigidbody2D.velocity = new Vector2(
                                Mathf.Clamp(
                                    _rigidbody2D.velocity.x - _moveConfiguration.DecelerationSpeed * Time.deltaTime, 
                                    _moveVelocity, 
                                    _rigidbody2D.velocity.x), 
                                _rigidbody2D.velocity.y);
                        }
                        break;
                }
            }
        }

        private void ApplyJump()
        {
            if (_rigidbody2D.velocity.y < 0f)
            {   // if vertical velocity is negative, then we must no longer be jumping and would be falling if we are not grounded
                _jumpState = _grounded ? JumpState.Grounded : JumpState.Falling;
                
                _sideScrollAnimation.Jumping(false);
                _sideScrollAnimation.Falling(_jumpState == JumpState.Falling);
            }

            if (_grounded)
            {
                _airJumpCount = 0;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event delegates
        
        private void MoveAction(object sender, EventArgs args)
        {
            _moveVelocity = ((MoveEventArgs) args).Value.x * _moveConfiguration.TopSpeed;
            _moveState = MoveState.Moving;
            _facing = _moveVelocity > 0f ? Direction.Right : Direction.Left;
            
            _sideScrollAnimation.Moving(_moveState == MoveState.Moving);
            if (_moveState == MoveState.Moving)
            {
                _sideScrollAnimation.Facing(_facing);
            }
        }

        private void StopAction(object sender, EventArgs args)
        {
            _moveVelocity = 0f;
            _moveState = MoveState.Idle;
            
            _sideScrollAnimation.Moving(_moveState == MoveState.Moving);
        }
        
        private void JumpAction(object sender, EventArgs args)
        {
            if (_grounded || (_jumpConfiguration.AirJump && _rigidbody2D.velocity.y > -_jumpConfiguration.AirJumpWindow && _rigidbody2D.velocity.y < _jumpConfiguration.AirJumpWindow && _airJumpCount < _jumpConfiguration.MaxAirJumps))
            {   // only allow jump if we are grounded or double jump is enabled and we are around the apex of the jump and we still have air jump charges available
                _airJumpCount += _grounded ? 0 : 1;
                _rigidbody2D.velocity += Vector2.up * _jumpConfiguration.JumpSpeed;
                _jumpState = JumpState.Jumping;
                
                _sideScrollAnimation.Jumping(true);
                _sideScrollAnimation.Falling(false);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // core unity behavior
        
        protected void Awake()
        {
            AdditionalAwake();
        }

        protected void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _sideScrollAnimation = _visual.GetComponent<SideScrollAnimation>();
            
            _moveState = MoveState.Idle;
            _facing = Direction.Right;
            _moveVelocity = 0f;
            
            _jumpState = JumpState.Grounded;
            _airJumpCount = 0;

            SideScrollGameInput.Instance.OnMoveAction += MoveAction;
            SideScrollGameInput.Instance.OnStopAction += StopAction;
            SideScrollGameInput.Instance.OnJumpAction += JumpAction;
            
            AdditionalStart();
        }
        
        protected void Update()
        {
            _grounded = IsGrounded();
            
            ApplyMove();
            ApplyJump();
            AdditionalUpdate();
        }
        
        protected void FixedUpdate()
        {
            AdditionalFixedUpdate();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // override these in subclass

        protected virtual void AdditionalStart() { }
        protected virtual void AdditionalAwake() { }
        protected virtual void AdditionalUpdate() { }
        protected virtual void AdditionalFixedUpdate() { }
    }
}