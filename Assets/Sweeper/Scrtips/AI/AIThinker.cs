using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(EnemyBoardObject))]
    public class AIThinker : MonoBehaviour
    {
        public EnemyBoardObject _boardObject;

        public PlayerBoardObject _player;

        //private IState _currentState;
        //private IState _prevState;
        private Stack<IState> _stateStack;

        private void Awake()
        {
            _boardObject = GetComponent<EnemyBoardObject>();
            _player = FindObjectOfType<PlayerBoardObject>();
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

        public void Think()
        {
            if (_stateStack.Count != 0)
            {
                _stateStack.Peek().Update();
            }
        }

        public void ChangeState(IState newState)
        {
            Debug.Log("State changed to " + newState.ToString());

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
