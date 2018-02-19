using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

public class BoardJumper : MonoBehaviour
{
    bool _jumping = false;

    Vector3 _targetPosition = Vector3.zero;
    Vector3 _middlePosition = Vector3.zero;
    Vector3 _startPosition = Vector3.zero;

    private float _jumpDistance;
    private float _jumpHeight = 1.0f;

    private Utils.Timer _timer = new Utils.Timer(1.0f);
    public float _speed = 1.0f;

    private void Update()
    {
        Jump();
        if (Input.GetKeyDown(KeyCode.G))
        {
            JumpBy(2, 0);
        }
    }

    public void JumpTo(int x, int z)
    {
        if (_jumping || !GameStateManager.Instance.BoardManager.CurrentBoard.CanMoveTo(x, z))
        {
            return;
        }

        _targetPosition = new Vector3(x, 0, z);
        _middlePosition = ((_targetPosition + _startPosition) / 2.0f) + new Vector3(0, _jumpHeight, 0);
        GameStateManager.Instance.RemoveExclamations();
        if (_targetPosition != _startPosition)
        {
            _jumping = true;
        }
    }
    public void JumpBy(int x, int z)
    {
        if (x == 0 && z == 0)
        {
            return;
        }
        Vector3 targetPos = _targetPosition + new Vector3(x, 0, z);
        JumpTo((int)targetPos.x, (int)targetPos.z);

    }

    private void Jump()
    {
        if (_jumping)
        {
            bool isTick = _timer.Tick(Time.deltaTime * _speed);

            float t = _timer.Percent;
            float oneMinusT = 1.0f - _timer.Percent;
            Vector3 currentPosition = (_startPosition * oneMinusT * oneMinusT) +
                                (2.0f * _middlePosition * oneMinusT * _timer.Percent) +
                                (_targetPosition * t * t);
            if (isTick)
            {
                currentPosition = _targetPosition;
                _startPosition = _targetPosition;
                _jumping = false;
                _timer.Reset();

                //만약 플레이어가 죽거나 출구에 도착한다면 true를 반환한다
                if (GameStateManager.Instance.CheckMovement((int)_targetPosition.x, (int)_targetPosition.z))
                {
                    return;
                }
            }
            transform.position = currentPosition;
        }
    }


}
