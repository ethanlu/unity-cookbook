using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity2dCookbook
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SideScrollAnimation : MonoBehaviour
    {
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

        public void Moving(bool m) { _moving = m; }
        public void Facing(Direction d) { _facing = d; }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _moving = false;
            _facing = Direction.Right;
        }
        
        private void Update()
        {
            _animator.SetBool(MOVING, _moving);
            _spriteRenderer.flipX = _facing == Direction.Left;
        }
    }
}