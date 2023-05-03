using System;
using System.Collections.Generic;

namespace Common.FSM
{
    public abstract class StateMachine
    {
        protected Dictionary<string, IState> _states;
        protected string _current;

        public StateMachine()
        {
            _states = new Dictionary<string, IState>();
            _current = "";
        }

        private IState GetState(string state)
        {
            if (!_states.ContainsKey(state))
            {   // lazy load state
                var path = StatePath();
                var state_class = Type.GetType($"{path}.{state}State, Assembly-CSharp");
                if (state_class is null)
                {
                    throw new Exception($"Could not load assembly : {path}.{state}State, Assembly-CSharp");
                }
                
                var s = Activator.CreateInstance(state_class, StateParameters()) as IState;
                if (s is null)
                {
                    throw new Exception($"Could not instantiate class : {path}.{state}State");
                }
                
                _states[state] = s;
            }

            return _states[state];
        }

        public IState Current()
        {
            return GetState(_current);
        }

        public void Start(string state)
        {
            _current = state;
            GetState(_current).Enter();
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
        
        public void Update()
        {
            GetState(_current).Update();
        }

        public void FixedUpdate()
        {
            GetState(_current).FixedUpdate();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // abstract methods

        public abstract string StatePath();
        public abstract object[] StateParameters();
    }
}