using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMover : MonoBehaviour
{
    private bool _moving;

    Vector3 _targetPosition = Vector3.zero;
    Vector3 _startPosition = Vector3.zero;

    private float _t;
    public float _speed = 1.0f;

    Cell[] _adjacentCells;

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
    }

    private void Move()
    {
        if (_moving)
        {
            _t += _speed * Time.deltaTime;

            Vector3 currentPosition = Vector3.Lerp(_startPosition, _targetPosition, _t);
            if (_t >= 0.99f)
            {
                currentPosition = _targetPosition;
                _startPosition = _targetPosition;
                _moving = false;
                _t = 0.0f;

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
                        GameStateManager.Instance.SpawnExclamation(currentCell.X, currentCell.Z);
                    }
                }
                count++;
            }
        }
    }

    private void UpdateAdjacentCells()
    {
        GameStateManager.Instance.CurrentBoard.GetAdjacentCells((int)_targetPosition.x, (int)_targetPosition.z, ref _adjacentCells);
    }
}
