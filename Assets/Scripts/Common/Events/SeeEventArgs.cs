using System;
using UnityEngine;

namespace Common.Events
{
    public class SeeEventArgs : EventArgs
    {
        public string source { get; set; }
        public Vector2 position { get; set; }
    }
}