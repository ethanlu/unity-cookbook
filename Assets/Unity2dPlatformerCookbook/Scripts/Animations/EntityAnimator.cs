using System;
using Unity2dPlatformerCookbook.Scripts.Utils;
using UnityEngine;

namespace Unity2dPlatformerCookbook.Scripts.Animations
{
    public class AnimationEventArgs : EventArgs
    {
        public string name { get; set; }
    }
    
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class EntityAnimator : MonoBehaviour
    {
        public event EventHandler OnAnimationEvent;
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// animator parameters
        private const string STARTING = "Starting";
        private const string ATTACKING = "Attacking";
        private const string FALLING = "Falling";
        private const string JUMPING = "Jumping";
        private const string MOVING = "Moving";
        private const string STUNNED = "Stunned";

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        
        private Direction _facing;
        private bool _moving;
        private bool _jumping;
        private bool _falling;
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// event delegates

        public void AnimationEvent(string name)
        {
            if (OnAnimationEvent is not null)
            {
                AnimationEventArgs args = new AnimationEventArgs();
                args.name = name;
                OnAnimationEvent(this, args);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// setters
        
        public void Moving(bool m)
        {
            _moving = m;
        }

        public void Jumping(bool j)
        {
            _jumping = j;
        }

        public void Falling(bool f)
        {
            _falling = f;
        }

        public void Facing(Direction d)
        {
            _facing = d;
        }

        public void Starting(bool s)
        {
            _animator.SetBool(STARTING, s);
        }

        public void Attack(int a)
        {
            _animator.SetInteger(ATTACKING, a);
        }

        public void PlayAnimation(string animation)
        {
            _animator.Play(animation);
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void Update()
        {
            _animator.SetBool(MOVING, _moving);
            _animator.SetBool(JUMPING, _jumping);
            _animator.SetBool(FALLING, _falling);
            _spriteRenderer.flipX = _facing == Direction.Left;
        }
    }
}