using System;
using Unity2dPlatformerCookbook.Scripts.Animations;
using Unity2dPlatformerCookbook.Scripts.Controls;
using Unity2dPlatformerCookbook.Scripts.Utils;
using UnityEngine;

namespace Unity2dPlatformerCookbook.Scripts.Entities
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

    [Serializable]
    public class AttackConfiguration
    {
        public AttackConfiguration()
        {
            AttackMove = false;
            ComboWindow = 1f;
        }

        public bool AttackMove;
        public float ComboWindow;
    }

    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Entity : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // customizable fields
        
        [SerializeField] protected GameObject _visual;
        [SerializeField] protected MoveConfiguration _moveConfiguration;
        [SerializeField] protected JumpConfiguration _jumpConfiguration;
        [SerializeField] protected AttackConfiguration _attackConfiguration;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // unity components
        
        protected BoxCollider2D _boxCollider2D;
        protected Rigidbody2D _rigidbody2D;
        
        protected EntityAnimator _entityAnimator;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // internal properties

        protected bool _grounded;
        
        // states
        protected Direction _facing;
        protected MoveState _moveState;
        protected JumpState _jumpState;
        protected AttackState _attackState;

        // move properties
        protected float _moveVelocity;
        
        // jump properties
        protected int _airJumpCount;
        
        // attack properties
        protected int _attackCombo;

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
                
                _entityAnimator.Jumping(false);
                _entityAnimator.Falling(_jumpState == JumpState.Falling);
            }

            if (_grounded)
            {
                _airJumpCount = 0;
            }
        }

        private void ApplyAttack()
        {
            switch (_attackState)
            {
                case AttackState.Ready:
                    // initial attack
                    _attackCombo++;
                    _attackState = AttackState.Attacking;
                    break;
                case AttackState.Recovery:
                    // recovery phase...can do combo if
                    if (_attackCombo == 1)
                    {
                        _attackCombo++;
                        _attackState = AttackState.Attacking;
                    }
                    break;
                default:
                    // invalid time to attack
                    _attackCombo = 0;
                    _attackState = AttackState.Ready;
                    break;
            }
            
            if (_attackState == AttackState.Attacking && !_attackConfiguration.AttackMove && _jumpState == JumpState.Grounded)
            {   // we are attacking, but attack move is not enabled and we are grounded....so set move velocity to 0 
                _moveVelocity = 0f;
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
            
            _entityAnimator.Moving(_moveState == MoveState.Moving);
            if (_moveState == MoveState.Moving)
            {
                _entityAnimator.Facing(_facing);
            }
        }

        private void StopAction(object sender, EventArgs args)
        {
            _moveVelocity = 0f;
            _moveState = MoveState.Idle;
            
            _entityAnimator.Moving(_moveState == MoveState.Moving);
        }
        
        private void JumpAction(object sender, EventArgs args)
        {
            if (_grounded || (_jumpConfiguration.AirJump && _rigidbody2D.velocity.y > -_jumpConfiguration.AirJumpWindow && _rigidbody2D.velocity.y < _jumpConfiguration.AirJumpWindow && _airJumpCount < _jumpConfiguration.MaxAirJumps))
            {   // only allow jump if we are grounded or double jump is enabled and we are around the apex of the jump and we still have air jump charges available
                _airJumpCount += _grounded ? 0 : 1;
                _rigidbody2D.velocity += Vector2.up * _jumpConfiguration.JumpSpeed;
                _jumpState = JumpState.Jumping;
                
                _entityAnimator.Jumping(true);
                _entityAnimator.Falling(false);
            }
        }

        private void AttackAction(object sender, EventArgs args)
        {
            
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
            _entityAnimator = _visual.GetComponent<EntityAnimator>();
            
            _moveState = MoveState.Idle;
            _facing = Direction.Right;
            _moveVelocity = 0f;
            
            _jumpState = JumpState.Grounded;
            _airJumpCount = 0;

            _attackState = AttackState.Ready;
            _attackCombo = 0;

            GameInput.Instance.OnMoveAction += MoveAction;
            GameInput.Instance.OnStopAction += StopAction;
            GameInput.Instance.OnJumpAction += JumpAction;
            GameInput.Instance.OnAttackAction += AttackAction;
            
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