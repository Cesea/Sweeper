﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

[RequireComponent(typeof(BoardMovementManager))]
public class BoardJump : BoardMoveBase
{
    Vector3 _middlePosition = Vector3.zero;

    public float _jumpHeight = 1.0f;
    private float _jumpDistance;

    private void Update()
    {
        Move();
    }

    public override void MoveBy(int x, int y, int z)
    {
        if (x == 0 && y == 0 && z == 0)
        {
            return;
        }

        NodeSideInfo targetNodeInfo = new NodeSideInfo();
        targetNodeInfo._node = BoardManager.Instance.CurrentBoard.GetOffsetedNode(_startNodeInfo._node, x, y, z);
        targetNodeInfo._side = _startNodeInfo._side;
        MoveTo(targetNodeInfo);
    }

    public override void MoveTo(NodeSideInfo info )
    {
        if (_moving || (info._node == null))
        {
            return;
        }

        _targetNodeInfo = info;
        _targetPosition = info._node.GetWorldPositionBySide(info._side);

        _middlePosition = ((_targetPosition + _startPosition) / 2.0f) + new Vector3(0, _jumpHeight, 0);

        if (_targetPosition != _startPosition)
        {
            _moving = true;
            _movementManager.OnMovementStart();
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

                _movementManager.OnMovementDone(_targetNodeInfo);
            }
            else
            {
                transform.position = currentPosition;
            }
        }
    }
}
