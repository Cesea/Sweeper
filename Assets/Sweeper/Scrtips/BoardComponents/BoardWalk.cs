using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardObject))]
public class BoardWalk : BoardMoveBase
{
    public override void MoveTo(int x, int y, int z)
    {
        if (_moving || !GameStateManager.Instance.BoardManager.CurrentBoard.CanMoveTo(x, y, z))
        {
            return;
        }

        _targetPosition = BoardManager.BoardPosToWorldPos(new Vector3Int(x, y, z));

        if (_targetPosition != _startPosition)
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
        Vector3Int tmpTarget = BoardManager.WorldPosToBoardPos(transform.position) + new Vector3Int(x, y, z);
        MoveTo((int)tmpTarget.x, (int)tmpTarget.y, (int)tmpTarget.z);
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

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            MoveUp();
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            MoveDown();
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            MoveRight();
        }

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

                _object.OnMovementDone();
                //만약 플레이어가 죽거나 출구에 도착한다면 true를 반환한다
                if (GameStateManager.Instance.CheckMovement(_targetPosition.ToVector3Int()))
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
