using System;
using UnityEngine;

namespace Common.Events
{
    public class ControlMoveEventArgs : EventArgs
    {
        public Vector2 Value { get; set; }
    }
}