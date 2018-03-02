using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(EnemyBoardObject))]
    public class AIThinker : MonoBehaviour
    {
        public EnemyBoardObject _boardObject;

        //private IState _currentState;
        //private IState _prevState;
        private Stack<IState> _stateStack;

        private void Awake()
        {
            _boardObject = GetComponent<EnemyBoardObject>();
            _stateStack = new Stack<IState>();
        }

        private void Start()
        {
            SetInitialState(new IdleState());
        }

        public void SetInitialState(IState newState)
        {
            ChangeState(newState);
        }

        private void Update()
        {
            if (_stateStack.Count != 0)
            {
                _stateStack.Peek().Update();
            }
        }

        public void ChangeState(IState newState)
        {
            if (newState != null)
            {
                //이전 상태가 없을때
                if (_stateStack.Count == 0)
                {
                    _stateStack.Push(newState);
                    newState.Enter(_boardObject);
                }
                //이전 상태가 있을때
                else
                {
                    IState prevState = _stateStack.Peek();
                    prevState.Exit();
                    _stateStack.Push(newState);
                    newState.Enter(_boardObject);
                }
            }
        }

        public void PopState()
        {
            if (_stateStack.Count != 0)
            {
                IState prevState = _stateStack.Pop();
                prevState.Exit();

                _stateStack.Peek().Enter(_boardObject);
            }
        }
    }
}
