using System;
using Unity2dPlatformerCookbook.Scripts.Animations;
using Unity2dPlatformerCookbook.Scripts.Controls;
using Unity2dPlatformerCookbook.Scripts.Entities.States;
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
            AerialMove = true;
            GroundDistance = .5f;
        }

        public LayerMask GroundLayer;
        public float JumpSpeed;
        public bool AirJump;
        public int MaxAirJumps;
        public float AirJumpWindow;
        public bool AerialMove;
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

        // states
        protected EntityStateMachine _stateMachine;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // internal methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // getters

        public EntityAnimator EntityAnimator() { return _entityAnimator; }
        public BoxCollider2D BoxCollider2D() { return _boxCollider2D; }
        public JumpConfiguration JumpConfiguration() { return _jumpConfiguration; }
        public MoveConfiguration MoveConfiguration() { return _moveConfiguration; }
        public AttackConfiguration AttackConfiguration() { return _attackConfiguration; }

        public Rigidbody2D Rigidbody2D() { return _rigidbody2D; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event delegates

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

            _stateMachine = new EntityStateMachine(this, EntityStateMachine.InitialState);

            AdditionalStart();
        }
        
        protected void Update()
        {
            _stateMachine.RunStateUpdate();

            AdditionalUpdate();
        }
        
        protected void FixedUpdate()
        {
            _stateMachine.RunStateFixedUpdate();
            
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