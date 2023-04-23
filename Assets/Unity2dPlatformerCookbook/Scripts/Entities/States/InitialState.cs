using System;
using Unity2dPlatformerCookbook.Scripts.Animations;
using Unity2dPlatformerCookbook.Scripts.Utils;
using UnityEngine;

namespace Unity2dPlatformerCookbook.Scripts.Entities.States
{
    public class InitialState : EntityState
    {
        public InitialState(Entity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
        {
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event delegates
        
        private void InitialAnimationFinish(object sender, EventArgs args)
        {
            if (((AnimationEventArgs) args).name == EntityStateMachine.InitialState)
            {   // initial animation finished...go to idle state
                _stateMachine.ChangeState(EntityStateMachine.GroundedState);
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

            _entity.EntityAnimator().Starting(true);
            _entity.EntityAnimator().Moving(false);
            _entity.EntityAnimator().Jumping(false);
            _entity.EntityAnimator().Facing(_facing);
            _entity.EntityAnimator().OnAnimationEvent += InitialAnimationFinish;
        }

        public override void Exit()
        {
            base.Exit();
            
            _entity.EntityAnimator().Starting(false);
            _entity.EntityAnimator().PlayAnimation("Idle");
            _entity.EntityAnimator().OnAnimationEvent -= InitialAnimationFinish;
        }

        public override void Update()
        {
            //base.Update();
        }

        public override void FixedUpdate()
        {
            //base.FixedUpdate();
        }
    }
}