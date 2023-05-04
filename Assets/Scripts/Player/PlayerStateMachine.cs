using Common.FSM;
using Player.Animations;
using Player.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerStateMachine : StateMachine
    {
        public const string InitialState = "Initial";
        public const string GroundedState = "Grounded";
        public const string AerialState = "Aerial";

        private object[] _parmameters;

        public PlayerStateMachine(
            Rigidbody2D physicsBody,
            BoxCollider2D physicsCollider,
            PlayerAnimator animator,
            PlayerHitBox hitBox,
            PlayerHurtBox hurtBox,
            MoveConfiguration moveConfiguration,
            JumpConfiguration jumpConfiguration,
            AttackConfiguration attackConfiguration
        ) : base()
        {
            _parmameters = new object[]{ this, physicsBody, physicsCollider, animator, hitBox, hurtBox, moveConfiguration, jumpConfiguration, attackConfiguration };
        }

        public override string StatePath()
        {
            return "Player.States";
        }
        
        public override object[] StateParameters()
        {
            return _parmameters;
        }
    }
}