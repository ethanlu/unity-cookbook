using Common.Events;
using System;
using Utils;
using UnityEngine;

namespace Dummy.Animations
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class DummyAnimator : MonoBehaviour
    {
        public const string HurtBox = "DummyHurtBox";
        
        public event EventHandler OnAnimationEvent;
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// animator parameters
        private const string HURT = "Hurt";

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private Direction _facing;

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// event delegates

        public void AnimationEvent(string value)
        {
            if (OnAnimationEvent is not null)
            {
                var parameters = value.Split(",");
                AnimationEventArgs args = new AnimationEventArgs();
                args.param1 = parameters[0];
                args.param2 = (parameters.Length > 1) ? parameters[1] : "";
                OnAnimationEvent(this, args);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// setters

        public void Facing(Direction d)
        {
            _facing = d;
            _spriteRenderer.flipX = _facing == Direction.Right;
        }

        public void Hurt()
        {
            _animator.SetTrigger(DummyAnimator.HURT);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // core unity behavior

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _facing = Direction.Left;
        }
    }
}