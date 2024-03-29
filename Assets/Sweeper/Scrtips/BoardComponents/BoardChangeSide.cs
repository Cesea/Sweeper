﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardMoveBase))]
public class BoardChangeSide : BoardMoveBase
{
    Vector3 _cornerPosition = Vector3.zero;

    public override void MoveTo(NodeSideInfo info)
    {
        if (_moving || (info._node == null) ||
            ((info._side == _startNodeInfo._side) && (info._node != _startNodeInfo._node)) ||
            ((info._side == _startNodeInfo._side) && (info._node == _startNodeInfo._node)))
        {
            return;
        }

        if (info._node == _startNodeInfo._node)
        {
            Vector3 startPosition = _startNodeInfo.GetWorldPosition();
            Vector3 diff = BoardManager.SideToVector3Offset(info._side);

            _cornerPosition = startPosition + diff;
        }
        else
        {
            _cornerPosition = NodeSideInfo.GetClosestEdge(_startNodeInfo, info);
        }

        _targetNodeInfo = info;
        _targetPosition = _targetNodeInfo._node.GetWorldPositionBySide(_targetNodeInfo._side);

        if (_targetPosition != _startPosition)
        {
            _moving = true;
            _movementManager.OnMovementStart();
        }
    }

    private void Update()
    {
        Move();
    }

    protected override void Move()
    {
        if (_moving)
        {
            bool isTick = _timer.Tick(Time.deltaTime * _speed);

            float percent = _timer.Percent;
            Vector3 currentPosition;
            if (percent < 0.5f)
            {
                currentPosition = Vector3.Lerp(_startPosition, _cornerPosition, _timer.Percent / 0.5f);
            }
            else
            {
                currentPosition = Vector3.Lerp(_cornerPosition, _targetPosition, _timer.Percent / 0.5f - 1.0f);
            }

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
