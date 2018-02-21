﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

[RequireComponent(typeof(BoardObject))]
public class BoardJump : BoardMoveBase
{
    Vector3 _middlePosition = Vector3.zero;

    public float _jumpHeight = 1.0f;
    private float _jumpDistance;

    private void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.G))
        {
            MoveBy(2, 0);
        }
    }
    public override void MoveBy(int x, int z)
    {
        if (x == 0 && z == 0)
        {
            return;
        }

        Vector2Int targetPos = BoardManager.WorldPosToBoardPos(transform.position) + new Vector2Int(x, z);
        MoveTo(targetPos.x, targetPos.y);
    }

    public override void MoveTo(int x, int z)
    {
        if (_moving || !GameStateManager.Instance.BoardManager.CurrentBoard.CanMoveTo(x, z))
        {
            return;
        }

        _targetPosition = BoardManager.BoardPosToWorldPos(new Vector2Int(x, z));
        _middlePosition = ((_targetPosition + _startPosition) / 2.0f) + new Vector3(0, _jumpHeight, 0);

        if (_targetPosition != _startPosition)
        {
            _moving = true;
            _object.OnMovementStart();
        }
    }

    protected override void Move()
    {
       if (_moving)
        {
            bool isTick = _timer.Tick(Time.deltaTime * _speed);

            float t = _timer.Percent;
            float oneMinusT = 1.0f - _timer.Percent;
            Vector3 currentPosition = (_startPosition * oneMinusT * oneMinusT) +
                                (2.0f * _middlePosition * oneMinusT * _timer.Percent) +
                                (_targetPosition * t * t);
            if (isTick)
            {
                transform.position = _targetPosition;
                _moving = false;
                _timer.Reset();

                _object.OnMovementDone();
                //만약 플레이어가 죽거나 출구에 도착한다면 true를 반환한다
                if (GameStateManager.Instance.CheckMovement((int)_targetPosition.x, (int)_targetPosition.z))
                {
                    return;
                }
            }
            else
            {
                transform.position = currentPosition;
            }
        }
    }
}
