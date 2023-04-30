using System;
using Utils;
using UnityEngine;

namespace Animations
{
    public class AnimationEventArgs : EventArgs
    {
        public string param1 { get; set; }
        public string param2 { get; set; }
    }
    
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class EntityAnimator : MonoBehaviour
    {
        public event EventHandler OnAnimationEvent;
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// animator parameters
        private const string STARTING = "Starting";
        private const string GROUNDATTACKING = "GroundAttacking";
        private const string AIRATTACKING = "AirAttacking";
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

        public void GroundAttacking(int a)
        {
            _animator.SetInteger(GROUNDATTACKING, a);
        }
        
        public void AirAttacking(int a)
        {
            _animator.SetInteger(AIRATTACKING, a);
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