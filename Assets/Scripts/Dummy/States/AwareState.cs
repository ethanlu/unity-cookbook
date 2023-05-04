using System;
using Common.FSM;
using Dummy.Animations;
using Utils;
using UnityEngine;

namespace Dummy.States
{
    public class AwareState : DummyState
    {
        private Vector2 _original;
        
        public AwareState(
            DummyStateMachine stateMachine,
            Rigidbody2D physicsBody,
            BoxCollider2D physicsCollider,
            DummyAnimator animator,
            DummyHitBox hitBox,
            DummyHurtBox hurtBox
        ) : base (stateMachine, physicsBody, physicsCollider, animator, hitBox, hurtBox)
        {
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event delegates

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // internal methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // interface methods

        public override void Enter()
        {
            base.Enter();
            
            // increase dummy hitbox size
            BoxCollider2D hitbox = _hitBox.GetComponent<BoxCollider2D>();
            _original = hitbox.size;
            hitbox.size = new Vector2(hitbox.size.x * 4f, hitbox.size.y * 4f);
         
            _hitBox.OnTrackEvent += SeeEvent;
            _hurtBox.OnHurtEvent += HurtEvent;
        }

        public override void Exit()
        {
            base.Exit();
            
            // decrease dummy hitbox size back to original
            BoxCollider2D hitbox = _hitBox.GetComponent<BoxCollider2D>();
            hitbox.size = _original;
            
            _hitBox.OnTrackEvent -= SeeEvent;
            _hurtBox.OnHurtEvent -= HurtEvent;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}