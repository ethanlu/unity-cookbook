using System;
using Common.Events;
using Player.Animations;
using Player.Data;
using Utils;
using UnityEngine;

namespace Player.States
{
    public class InitialState : PlayerState
    {
        public InitialState(
            PlayerStateMachine stateMachine,
            Rigidbody2D physicsBody,
            BoxCollider2D physicsCollider,
            PlayerAnimator animator,
            MoveConfiguration moveConfiguration,
            JumpConfiguration jumpConfiguration,
            AttackConfiguration attackConfiguration
        ) : base(stateMachine, physicsBody, physicsCollider, animator, moveConfiguration, jumpConfiguration, attackConfiguration)
        {
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event delegates
        
        private void InitialAnimationFinish(object sender, EventArgs args)
        {
            if (((AnimationEventArgs) args).param1 == PlayerStateMachine.InitialState)
            {   // initial animation finished...go to idle state
                _stateMachine.ChangeState(PlayerStateMachine.GroundedState);
            }
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // interface methods

        public override void Enter()
        {
            base.Enter();

            _moveVelocity = 0f;
            _jumpVelocity = 0f;
            _airJumpCount = 0;
            _facing = Direction.Right;
            _attackSequence = 0;

            _animator.Starting(true);
            _animator.Moving(false);
            _animator.Jumping(false);
            _animator.Facing(_facing);
            _animator.OnAnimationEvent += InitialAnimationFinish;
        }

        public override void Exit()
        {
            base.Exit();
            
            _animator.Starting(false);
            _animator.PlayAnimation("Idle");
            _animator.OnAnimationEvent -= InitialAnimationFinish;
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