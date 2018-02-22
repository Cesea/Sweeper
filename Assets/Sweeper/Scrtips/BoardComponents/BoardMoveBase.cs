using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardObject))]
public class BoardMoveBase : MonoBehaviour
{
    protected bool _moving;

    protected Vector3 _targetPosition = Vector3.zero;
    protected Vector3 _startPosition = Vector3.zero;

    protected Utils.Timer _timer = new Utils.Timer(1.0f);
    public float _speed = 1.0f;

    protected BoardObject _object;

    protected virtual void Awake()
    {
        _object = GetComponent<BoardObject>();
    }


    protected virtual void Move()
    {
    }

    public virtual void MoveTo(int x, int y, int z)
    {
    }

    public virtual void MoveBy(int x, int y, int z)
    {
        if (x == 0 && y == 0 && z == 0)
        {
            return;
        }
    }

    public virtual void UpdatePositionInfos(Vector3 position)
    {
        _targetPosition = position;
        _startPosition = position;
    }
}
