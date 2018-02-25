using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardObject))]
public class BoardWalk : BoardMoveBase
{
    public override void MoveTo(NodeSideInfo info)
    {
        if (_moving || (info.Node == null) )
        {
            return;
        }

        //갈 수 있는곳인지 없는곳 인지 판단
        if (info.Side == Side.Top)
        {
            Node nodeRespecteSide = BoardManager.Instance.CurrentBoard.GetOffsetedNode(info.Node, 0, 1, 0);
            //갈 수 없는 곳 이다.
            if (nodeRespecteSide.IsSolid)
            {
                return;
            }
        }

        _targetNodeInfo = info;
        _targetPosition = _targetNodeInfo.Node.GetWorldPositionBySide(_targetNodeInfo.Side);

        if (_targetPosition != _sittingPosition)
        {
            _moving = true;
            _object.OnMovementStart();
        }
    }

    public override void MoveBy(int x, int y, int z)
    {
        if ((x == 0 && z == 0))
        {
            return;
        }

        NodeSideInfo targetNodeInfo = new NodeSideInfo();
        targetNodeInfo.Node = BoardManager.Instance.CurrentBoard.GetOffsetedNode(_sittingNodeInfo.Node, x, y, z);
        targetNodeInfo.Side = _sittingNodeInfo.Side;

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
            Vector3 currentPosition = Vector3.Lerp(_sittingPosition, _targetPosition, _timer.Percent);
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
