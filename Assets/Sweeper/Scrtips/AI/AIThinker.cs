using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(EnemyBoardObject))]
    public class AIThinker : MonoBehaviour
    {
        private EnemyBoardObject _boardObject;

        private IState _currentState;
        private IState _prevState;

        private void Awake()
        {
            _boardObject = GetComponent<EnemyBoardObject>();
        }

        private void Update()
        {
        }

        private void ChangeState(IState newState)
        {

        }

        private void PopState()
        {

        }
    }
}
