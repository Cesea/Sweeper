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

    public void MoveToCell(int x, int z)
    {
        if (_moving || !GameStateManager.Instance.BoardManager.CurrentBoard.CanMoveTo(x, z))
        {
            return;
        }

        _targetPosition = new Vector3(x, 0, z);

        if (_targetPosition != _startPosition)
        {
            _moving = true;
        }
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

                //Check board
                GameStateManager.Instance.CheckMovement((int)_targetPosition.x, (int)_targetPosition.z);

            }
            transform.position = currentPosition;
        }
    }
}
