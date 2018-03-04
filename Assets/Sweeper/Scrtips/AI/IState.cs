using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public interface IState
    {
        void Enter(EnemyBoardObject owner);
        void Update();
        void Exit();

        void CheckStatus();
    }
}
