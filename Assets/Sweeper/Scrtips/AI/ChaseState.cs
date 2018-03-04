using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class ChaseState : IState
    {
        private EnemyBoardObject _owner;
        private AIThinker _thinker;

        private PlayerBoardObject _player;

        private float _seekDistance = 3.0f; 

        public void Enter(EnemyBoardObject owner)
        {
            _owner = owner;
            _thinker = _owner.GetComponent<AIThinker>();

            _player = _owner._playerObject;
        }

        public void Exit()
        {
        }

        public void Update()
        {
            Debug.Log("Chase State updating");
            Vector3 playerPosition = _player.transform.position;
            Vector3 enemyPosition = _owner.transform.position;

            _owner.MovementManager.StartFindPath(_player.SittingNode);

            //Distance between Player is too far change to idle state
            if (Vector3.Distance( playerPosition, enemyPosition) > _seekDistance)
            {
                _thinker.ChangeState(new IdleState());
                return;
            }
        }
    }
}
