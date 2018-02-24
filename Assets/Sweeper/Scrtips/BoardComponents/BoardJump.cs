using System.Collections;
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
    }

    public override void MoveBy(int x, int y, int z)
    {
        if (x == 0 && y == 0 && z == 0)
        {
            return;
        }

        NodeSideInfo targetNodeInfo = new NodeSideInfo();
        targetNodeInfo.Node = BoardManager.Instance.CurrentBoard.GetOffsetedNode(_sittingNodeInfo.Node, x, y, z);
        targetNodeInfo.Side = _sittingNodeInfo.Side;
        MoveTo(targetNodeInfo);
    }

    public override void MoveTo(NodeSideInfo info )
    {
        if (_moving || (info.Node == null))
        {
            return;
        }

        _targetNodeInfo = info;
        _targetPosition = info.Node.GetWorldPositionBySide(info.Side);

        _middlePosition = ((_targetPosition + _sittingPosition) / 2.0f) + new Vector3(0, _jumpHeight, 0);

        if (_targetPosition != _sittingPosition)
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
            Vector3 currentPosition = (_sittingPosition * oneMinusT * oneMinusT) +
                                (2.0f * _middlePosition * oneMinusT * _timer.Percent) +
                                (_targetPosition * t * t);
            if (isTick)
            {
                transform.position = _targetPosition;
                _moving = false;
                _timer.Reset();

                _object.OnMovementDone(_targetNodeInfo);
            }
            else
            {
                transform.position = currentPosition;
            }
        }
    }
}
