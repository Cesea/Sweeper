using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class IdleState : IState
    {
        private EnemyBoardObject _owner;
        private PlayerBoardObject _player;

        public void Enter(EnemyBoardObject owner)
        {
            _owner = owner;
            _player = _owner._playerObject;
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}
