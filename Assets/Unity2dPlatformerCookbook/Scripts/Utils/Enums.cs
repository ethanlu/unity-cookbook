using System.Collections;
using System.Collections.Generic;

namespace Unity2dPlatformerCookbook.Scripts.Utils
{
    public enum Direction
    {
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3
    }

    public enum MoveState
    {
        Idle = 0,
        Moving = 1
    }

    public enum JumpState
    {
        Grounded = 0,
        Jumping = 1,
        Falling = 2,
    }

    public enum AttackState
    {
        Ready = 0,
        Attacking = 1,
        Recovery = 2
    }
}