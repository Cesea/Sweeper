using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMovementManager : MonoBehaviour
{
    public Transform _targetTransform;

    private NodeSideInfo[] _path;
    private int _targetIndex = 0;

    public List<BoardMoveBase> _availableMovements = new List<BoardMoveBase>();

    NodeSideInfo _sittingNodeInfo;

    private void Start()
    {
        _sittingNodeInfo = new NodeSideInfo();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NodeSideInfo start = _sittingNodeInfo;
            NodeSideInfo end = new NodeSideInfo(BoardManager.Instance.CurrentBoard.GetNodeAt(_targetTransform.position), Side.Top);

            PathRequestManager.RequestPath(start, end, OnPathFind);
        }
    }

    public void SetSittingNode(Node node, Side side)
    {
        _sittingNodeInfo._node = node;
        _sittingNodeInfo._side = side;
        transform.position = _sittingNodeInfo.GetWorldPosition();
        OnMovementDone(_sittingNodeInfo);
    }

    public void OnMovementStart()
    {
        GameStateManager.Instance.RemoveExclamations();
    }

    public void OnMovementDone(NodeSideInfo info)
    {
        _sittingNodeInfo = info;
        //UpdateNeighbourCells();

        //만약 플레이어가 죽거나 출구에 도착한다면 true를 반환한다
        //if (GameStateManager.Instance.CheckMovement(_sittingNodeInfo._node))
        //{
        //    return;
        //}
        //CheckAdjacentCells();
        foreach (var movement in _availableMovements)
        {
            movement.UpdateNodeSideInfo(_sittingNodeInfo);
        }
    }

    public void OnPathFind(NodeSideInfo[] nodeInfos, bool success)
    {
        _path = nodeInfos;
        if (success)
        {
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        NodeSideInfo currentNode = _path[0];
        while (true)
        {
            if (transform.position == _sittingNodeInfo.GetWorldPosition())
            {
                _targetIndex++;
                if (_targetIndex >= _path.Length)
                {
                    _targetIndex = 0;
                    yield break;
                }
                currentNode = _path[_targetIndex];
                _availableMovements[0].MoveTo(currentNode);
            }
            yield return null;
        }
    }
}
