using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class IState
    {
        protected EnemyBoardObject _owner;

        public virtual void Enter(EnemyBoardObject owner)
        {
            _owner = owner;
        }

        public virtual void Update()
        {
        }

        public virtual void Exit()
        {
        }

    }
}
