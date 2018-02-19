using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardObject))]
public class BoardWalk : BoardMoveBase
{
    public override void MoveTo(int x, int z)
    {
        if (_moving || !GameStateManager.Instance.BoardManager.CurrentBoard.CanMoveTo(x, z))
        {
            return;
        }

        _targetPosition = new Vector3(x, 0, z);

        if (_targetPosition != _startPosition)
        {
            _moving = true;
            _object.OnMovementStart();
        }
    }

    public override void MoveBy(int x, int z)
    {
        if ((x == 0 && z == 0))
        {
            return;
        }
        Vector2Int tmpTarget = transform.WorldPosToCellPos() + new Vector2Int(x, z);
        MoveTo((int)tmpTarget.x, (int)tmpTarget.y);
    }


    public void MoveRight()
    {
        Vector3 targetPos = transform.position + Vector3.right;
        MoveTo((int)targetPos.x, (int)targetPos.z);
    }

    public void MoveLeft()
    {
        Vector3 targetPos = transform.position + Vector3.left;
        MoveTo((int)targetPos.x, (int)targetPos.z);
    }

    public void MoveUp()
    {
        Vector3 targetPos = transform.position + Vector3.forward;
        MoveTo((int)targetPos.x, (int)targetPos.z);
    }

    public void MoveDown()
    {
        Vector3 targetPos = transform.position + Vector3.back;
        MoveTo((int)targetPos.x, (int)targetPos.z);
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
