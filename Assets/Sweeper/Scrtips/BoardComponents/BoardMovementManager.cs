using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardObject))]
public class BoardMovementManager : MonoBehaviour
{
    private NodeSideInfo[] _path;
    private int _targetIndex = 0;

    public List<BoardMoveBase> _availableMovements = new List<BoardMoveBase>();

    public NodeSideInfo _sittingNodeInfo = null;

    private BoardObject _boardObject;

    private bool _followingDone = true;

    public bool FollowingDone
    {
        get { return _followingDone; }
    }

    private void Awake()
    {
        _boardObject = GetComponent<BoardObject>();
    }

    public void SetSittingNode(int x, int y, int z, Side side)
    {
        if (!Object.ReferenceEquals(_sittingNodeInfo, null))
        {
            _sittingNodeInfo.SittingObject = null;
        }
        _sittingNodeInfo = BoardManager.Instance.CurrentBoard.GetNodeInfoAt(x, y, z, side);
        _sittingNodeInfo.SittingObject = gameObject;

        transform.position = _sittingNodeInfo.GetWorldPosition();
        OnMovementDone(_sittingNodeInfo);
    }

    public void OnMovementStart()
    {
        _sittingNodeInfo.SittingObject = null;
    }

    public void OnMovementDone(NodeSideInfo info)
    {
        _sittingNodeInfo = info;
        _sittingNodeInfo.SittingObject = gameObject;

        _boardObject.UpdateNeighbourCells();

        //만약 플레이어가 죽거나 출구에 도착한다면 true를 반환한다
        if (_boardObject.CheckMovement(_sittingNodeInfo))
        {
            StopFindPath();
            return;
        }

        foreach (var movement in _availableMovements)
        {
            movement.UpdateNodeSideInfo(_sittingNodeInfo);
        }

        if (_boardObject.CheckAdjacentCells())
        {
            StopFindPath();
        }

        _boardObject.OnMovementDone();

    }


    public void StopFindPath()
    {
        _targetIndex = 0;
        StopCoroutine("FollowPath");
        _followingDone = true;
    }

    public void StartFindPath(NodeSideInfo target)
    {
        PathRequestManager.RequestPath(_sittingNodeInfo, target, OnPathFind);
    }

    public void OnPathFind(NodeSideInfo[] nodeInfos, bool success)
    {
        _path = nodeInfos;
        if (success)
        {
            _followingDone = false;

            StopFindPath();
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        NodeSideInfo currentNodeInfo = _path[0];
        if (_path.Length == 1)
        {
            BoardMoveBase toUseMovement = null;
            currentNodeInfo = _path[_targetIndex];
            if (_sittingNodeInfo._side != currentNodeInfo._side)
            {
                toUseMovement = _availableMovements[1];
            }
            else
            {
                toUseMovement = _availableMovements[0];
            }
            toUseMovement.MoveTo(currentNodeInfo);
        }
        else
        {
            while (true)
            {
                if (transform.position == _sittingNodeInfo.GetWorldPosition())
                {
                    BoardMoveBase toUseMovement = null;

                    currentNodeInfo = _path[_targetIndex];
                    if (_sittingNodeInfo._side != currentNodeInfo._side)
                    {
                        toUseMovement = _availableMovements[1];
                    }
                    else
                    {
                        toUseMovement = _availableMovements[0];
                    }
                    toUseMovement.MoveTo(currentNodeInfo);

                    _targetIndex++;
                    if (_targetIndex >= _path.Length)
                    {
                        _targetIndex = 0;
                        yield break;
                    }
                }
                yield return null;
            }
        }

        _followingDone = true;
    }
}
