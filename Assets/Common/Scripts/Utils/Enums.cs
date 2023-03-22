using System.Collections;
using System.Collections.Generic;

namespace Unity2dCookbook
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
}