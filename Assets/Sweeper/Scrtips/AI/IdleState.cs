using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class IdleState : IState
    {
        private EnemyBoardObject _owner;
        private AIThinker _thinker;

        private PlayerBoardObject _player;
        
        private float _seekDistance = 3;

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
            Debug.Log("idle state updating");
        }

        public void CheckStatus()
        {
            Vector3 playerPosition = _player.transform.position;
            Vector3 enemyPosition = _owner.transform.position;

            if (Vector3.Distance(playerPosition, enemyPosition) < _seekDistance)
            {
                _thinker.ChangeState(new ChaseState());
                EventManager.Instance.TriggerEvent(new Events.EnemyChaseEvent(_owner.EnemyID));
            }
        }
    }
}
