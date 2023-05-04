using System;

namespace Common.Events
{
    public class HurtEventArgs : EventArgs
    {
        public string source { get; set; }
    }
}