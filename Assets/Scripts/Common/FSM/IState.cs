using System;

namespace Common.FSM
{
    public interface IState
    {
        public void Enter();
        public void Exit();
        public void Update();
        public void FixedUpdate();
    }
}