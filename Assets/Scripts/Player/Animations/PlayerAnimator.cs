using Common.Events;
using System;
using Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace Player.Animations
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAnimator : MonoBehaviour
    {
        public const string HitBox = "PlayerHitBox";
        
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
        private Transform _hitboxTransform;
        
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
            _animator.SetBool(MOVING, _moving);
        }

        public void Jumping(bool j)
        {
            _jumping = j;
            _animator.SetBool(JUMPING, _jumping);
        }

        public void Falling(bool f)
        {
            _falling = f;
            _animator.SetBool(FALLING, _falling);
        }

        public void Facing(Direction d)
        {
            if (_facing != d)
            {
                _hitboxTransform.localScale = new Vector3(_hitboxTransform.localScale.x * -1f, _hitboxTransform.localScale.y, _hitboxTransform.localScale.z);
            }
            
            _facing = d;
            _spriteRenderer.flipX = _facing == Direction.Left;
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
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // core unity behavior

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _hitboxTransform = transform.Find(PlayerAnimator.HitBox).GameObject().GetComponent<Transform>();

            _facing = Direction.Right;
        }
    }
}