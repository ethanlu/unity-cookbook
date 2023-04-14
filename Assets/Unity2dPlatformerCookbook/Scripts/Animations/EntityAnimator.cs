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
        public event EventHandler OnAnimationFinish;
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// animator parameters
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

        public void AnimationFinished(string name)
        {
            if (OnAnimationFinish is not null)
            {
                AnimationEventArgs args = new AnimationEventArgs();
                args.name = name;
                OnAnimationFinish(this, args);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// setters
        
        public void Moving(bool m)
        {
            _moving = m;
        }

        public void Jumping(bool m)
        {
            _jumping = m;
        }

        public void Falling(bool m)
        {
            _falling = m;
        }

        public void Facing(Direction d)
        {
            _facing = d;
        }

        public void PlayAnimation(string animation)
        {
            _animator.Play(animation);
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _moving = false;
            _jumping = false;
            _falling = false;
            _facing = Direction.Right;
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