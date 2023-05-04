using Common.FSM;
using Dummy.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dummy
{
    public class DummyStateMachine : StateMachine
    {
        public const string NormalState = "Normal";
        public const string AwareState = "Aware";

        private object[] _parmameters;

        public DummyStateMachine(
            Rigidbody2D physicsBody,
            BoxCollider2D physicsCollider,
            DummyAnimator animator,
            DummyHitBox hitBox,
            DummyHurtBox hurtBox
        ) : base()
        {
            _parmameters = new object[]{ this, physicsBody, physicsCollider, animator, hitBox, hurtBox};
        }

        public override string StatePath()
        {
            return "Dummy.States";
        }
        
        public override object[] StateParameters()
        {
            return _parmameters;
        }
    }
}