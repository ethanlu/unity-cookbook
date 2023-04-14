using System;
using Unity2dPlatformerCookbook.Scripts.Animations;
using UnityEngine;

namespace Unity2dPlatformerCookbook.Scripts.Entities.States
{
    public class InitialState : EntityState
    {
        public InitialState(Entity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
        {
        }
        
        private void InitialAnimationFinish(object sender, EventArgs args)
        {
            if (((AnimationEventArgs) args).name == EntityStateMachine.InitialState)
            {   // initial animation finished...go to idle state
                _stateMachine.ChangeState(EntityStateMachine.GroundedState);
            }
        }

        public override void Enter()
        {
            base.Enter();
            
            _entity.EntityAnimator().OnAnimationFinish += InitialAnimationFinish;
        }

        public override void Exit()
        {
            base.Exit();
            
            _entity.EntityAnimator().PlayAnimation("Idle");
            _entity.EntityAnimator().OnAnimationFinish -= InitialAnimationFinish;
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