using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMover : MonoBehaviour
{
    private bool _moving;

    Vector3 _targetPosition = Vector3.zero;
    Vector3 _startPosition = Vector3.zero;

    private Utils.Timer _timer = new Utils.Timer(1.0f);
    public float _speed = 1.0f;

    public GameObject _dangerSignPrefab;

    private Cell[] _adjacentCells;
    public Cell[] AdjacentCells
    {
        get { return _adjacentCells; }
    }

    private void Start()
    {
        _adjacentCells = new Cell[8];
    }

    public void MoveToCell(int x, int z)
    {
        if (_moving || !GameStateManager.Instance.BoardManager.CurrentBoard.CanMoveTo(x, z))
        {
            return;
        }

        _targetPosition = new Vector3(x, 0, z);
        GameStateManager.Instance.RemoveExclamations();

        if (_targetPosition != _startPosition)
        {
            _moving = true;
        }
    }

    public void MoveBy(int x, int z)
    {
        if ((x == 0 && z == 0))
        {
            return;
        }
        Vector3 tmpTarget = transform.position + new Vector3(x, 0, z);
        MoveToCell((int)tmpTarget.x, (int)tmpTarget.z);
    }

    public void SetPosition(Vector2 v)
    {
        transform.position = new Vector3(v.x, 0, v.y);
        _startPosition = transform.position;
        _targetPosition = _startPosition;
    }
    public void SetPosition(int x, int z)
    {
        transform.position = new Vector3(x, 0, z);
        _startPosition = transform.position;
        _targetPosition = _startPosition;
    }
    public void SetPosition(Vector2Int v)
    {
        transform.position= new Vector3(v.x, 0, v.y);
        _startPosition = transform.position;
        _targetPosition = _startPosition;
    }

    public void MoveRight()
    {
        Vector3 targetPos = transform.position + Vector3.right;
        MoveToCell((int)targetPos.x, (int)targetPos.z);
    }

    public void MoveLeft()
    {
        Vector3 targetPos = transform.position + Vector3.left;
        MoveToCell((int)targetPos.x, (int)targetPos.z);
    }

    public void MoveUp()
    {
        Vector3 targetPos = transform.position + Vector3.forward;
        MoveToCell((int)targetPos.x, (int)targetPos.z);
    }

    public void MoveDown()
    {
        Vector3 targetPos = transform.position + Vector3.back;
        MoveToCell((int)targetPos.x, (int)targetPos.z);
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BuildDangerSign((int)transform.position.x, (int)transform.position.z);
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

    public void BuildDangerSign(int x, int z)
    {
        GameObject go = Instantiate(_dangerSignPrefab);
        go.transform.position = new Vector3(x, 0, z);
    }

    private void Move()
    {
        if (_moving)
        {
            bool isTick = _timer.Tick(Time.deltaTime * _speed);
            Vector3 currentPosition = Vector3.Lerp(_startPosition, _targetPosition, _timer.Percent);
            if (isTick)
            {
                currentPosition = _targetPosition;
                _startPosition = _targetPosition;
                _moving = false;
                _timer.Reset();

                UpdateAdjacentCells();
                CheckAdjacentCells();

                //만약 플레이어가 죽거나 출구에 도착한다면 true를 반환한다
                if (GameStateManager.Instance.CheckMovement((int)_targetPosition.x, (int)_targetPosition.z))
                {
                    return;
                }
            }
            transform.position = currentPosition;
        }
    }

    private void CheckAdjacentCells()
    {
        bool isHazardExist = false;
        int count = 0;
        for (int z = 0; z < 3; ++z)
        {
            for (int x = 0; x < 3; ++x)
            {
                if (x == 1 && z == 1)
                {
                    continue;
                }
                Cell currentCell = _adjacentCells[count];
                if (currentCell != null)
                {
                    if (currentCell.Type == Cell.CellType.Mine)
                    {
                        isHazardExist = true;
                    }
                }
                count++;
            }
        }

        if (isHazardExist)
        {
            GameStateManager.Instance.SpawnExclamation((int)_startPosition.x, (int)_startPosition.z);
        }
    }

    private void UpdateAdjacentCells()
    {
        GameStateManager.Instance.CurrentBoard.GetAdjacentCells((int)_targetPosition.x, (int)_targetPosition.z, ref _adjacentCells);
    }
}
