using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class PatrolState : IState
    {
        EnemyBoardObject _owner;
        public void Enter(EnemyBoardObject owner)
        {
            _owner = owner;
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}
