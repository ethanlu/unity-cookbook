using System;
using Player.Animations;
using Utils;
using UnityEngine;

namespace Player.States
{
    public class InitialState : PlayerState
    {
        public InitialState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
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

            _player.PlayerAnimator().Starting(true);
            _player.PlayerAnimator().Moving(false);
            _player.PlayerAnimator().Jumping(false);
            _player.PlayerAnimator().Facing(_facing);
            _player.PlayerAnimator().OnAnimationEvent += InitialAnimationFinish;
        }

        public override void Exit()
        {
            base.Exit();
            
            _player.PlayerAnimator().Starting(false);
            _player.PlayerAnimator().PlayAnimation("Idle");
            _player.PlayerAnimator().OnAnimationEvent -= InitialAnimationFinish;
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