using System;

namespace Entities.States
{
    public interface IEntityState
    {
        public void Enter();
        public void Exit();
        public void Update();
        public void FixedUpdate();
    }
}