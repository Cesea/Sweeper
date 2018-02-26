using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardMovementManager))]
public class BoardMoveBase : MonoBehaviour
{
    protected bool _moving;

    protected Vector3 _targetPosition = Vector3.zero;
    protected Vector3 _startPosition = Vector3.zero;

    protected NodeSideInfo _startNodeInfo;
    protected NodeSideInfo _targetNodeInfo;

    protected Utils.Timer _timer = new Utils.Timer(1.0f);
    public float _speed = 1.0f;

    protected BoardMovementManager _movementManager;

    protected virtual void Awake()
    {
        _movementManager = GetComponent<BoardMovementManager>();
    }

    protected virtual void Move()
    {
    }

    public virtual void MoveTo(NodeSideInfo info)
    {
    }

    public virtual void MoveBy(int x, int y, int z)
    {
        if (x == 0 && y == 0 && z == 0)
        {
            return;
        }
    }

    public virtual void UpdateNodeSideInfo(NodeSideInfo newInfo)
    {
        _startNodeInfo = newInfo;

        _targetPosition = _startNodeInfo.GetWorldPosition();
        _startPosition = _startNodeInfo.GetWorldPosition();
    }
}
