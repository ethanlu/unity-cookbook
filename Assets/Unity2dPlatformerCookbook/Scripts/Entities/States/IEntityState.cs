using System;
using Unity2dPlatformerCookbook.Scripts.Controls;

namespace Unity2dPlatformerCookbook.Scripts.Entities.States
{
    public interface IEntityState
    {
        public void Enter();
        public void Exit();
        public void Update();
        public void FixedUpdate();
    }
}