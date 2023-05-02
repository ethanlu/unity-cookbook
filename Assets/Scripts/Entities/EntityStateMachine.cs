using System;
using System.Collections.Generic;
using Entities.States;
using UnityEngine;

namespace Entities
{
    public class EntityStateMachine
    {
        public const string InitialState = "Initial";
        public const string GroundedState = "Grounded";
        public const string AerialState = "Aerial";

        private Dictionary<string, IEntityState> _states;
        private string _current;

        private Entity _entity;

        public EntityStateMachine(Entity entity, string currentState)
        {
            _states = new Dictionary<string, IEntityState>();
            _current = currentState;
            _entity = entity;
            
            GetState(_current).Enter();
        }

        private IEntityState GetState(string state)
        {
            if (!_states.ContainsKey(state))
            {   // lazy load state
                var state_class = Type.GetType($"Entities.States.{state}State, Assembly-CSharp");
                if (state_class is null)
                {
                    throw new Exception($"Could not load assembly : Entities.States.{state}State, Assembly-CSharp");
                }

                object[] parameters = {_entity, this};
                var s = Activator.CreateInstance(state_class, parameters) as IEntityState;
                if (s is null)
                {
                    throw new Exception($"Could not instantiate class : Entities.States.{state}State");
                }
                _states[state] = s;
            }

            return _states[state];
        }

        public IEntityState Current()
        {
            return GetState(_current);
        }

        public void ChangeState(string newState)
        {
            if (_current != newState)
            {
                GetState(_current).Exit();
                _current = newState;
                GetState(_current).Enter();
            }
        }
        
        public void RunStateUpdate()
        {
            GetState(_current).Update();
        }

        public void RunStateFixedUpdate()
        {
            GetState(_current).FixedUpdate();
        }
    }
}