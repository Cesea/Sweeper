using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class FleeState : IState
    {
        private EnemyBoardObject _owner;
        private AIThinker _thinker;

        public void Enter(EnemyBoardObject owner)
        {
            _owner = owner;
            _thinker = _owner.GetComponent<AIThinker>();
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}
