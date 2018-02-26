using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardMovementManager))]
public class BoardWalk : BoardMoveBase
{
    public override void MoveTo(NodeSideInfo info)
    {
        if (_moving || (info._node == null) )
        {
            return;
        }

        //갈 수 있는곳인지 없는곳 인지 판단
        if (info._side == Side.Top)
        {
            Node nodeRespecteSide = BoardManager.Instance.CurrentBoard.GetOffsetedNode(info._node, 0, 1, 0);
            //갈 수 없는 곳 이다.
            if (nodeRespecteSide.IsSolid)
            {
                return;
            }
        }

        _targetNodeInfo = info;
        _targetPosition = _targetNodeInfo._node.GetWorldPositionBySide(_targetNodeInfo._side);

        if (_targetPosition != _startPosition)
        {
            _moving = true;
            _movementManager.OnMovementStart();
        }
    }

    public override void MoveBy(int x, int y, int z)
    {
        if ((x == 0 && z == 0))
        {
            return;
        }

        NodeSideInfo targetNodeInfo = new NodeSideInfo();
        targetNodeInfo._node = BoardManager.Instance.CurrentBoard.GetOffsetedNode(_startNodeInfo._node, x, y, z);
        targetNodeInfo._side = _startNodeInfo._side;

        MoveTo(targetNodeInfo);
    }


    public void MoveRight()
    {
        Vector3 targetPos = transform.position + Vector3.right;
        MoveBy(1, 0, 0);
    }

    public void MoveLeft()
    {
        Vector3 targetPos = transform.position + Vector3.left;
        MoveBy(-1, 0, 0);
    }

    public void MoveUp()
    {
        Vector3 targetPos = transform.position + Vector3.forward;
        MoveBy(0, 0, 1);
    }

    public void MoveDown()
    {
        Vector3 targetPos = transform.position + Vector3.back;
        MoveBy(0, 0, -1);
    }

    private void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            UseRadarSkill();
        }
    }

    //Temp
    public void UseRadarSkill()
    {
        EventManager.Instance.TriggerEvent(new Events.RadarSkillEvent(transform.position));
    }

    protected override void Move()
    {
        if (_moving)
        {
            bool isTick = _timer.Tick(Time.deltaTime * _speed);
            Vector3 currentPosition = Vector3.Lerp(_startPosition, _targetPosition, _timer.Percent);
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
