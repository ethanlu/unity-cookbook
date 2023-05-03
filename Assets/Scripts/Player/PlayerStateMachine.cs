using System;
using System.Collections.Generic;
using Common.FSM;
using UnityEngine;

namespace Player
{
    public class PlayerStateMachine : StateMachine
    {
        public const string InitialState = "Initial";
        public const string GroundedState = "Grounded";
        public const string AerialState = "Aerial";

        private Player _player;

        public PlayerStateMachine(Player entity) : base()
        {
            _player = entity;
        }

        public override string StatePath()
        {
            return "Player.States";
        }
        
        public override object[] StateParameters()
        {
            object[] parameters = {_player, this};
            return parameters;
        }
    }
}