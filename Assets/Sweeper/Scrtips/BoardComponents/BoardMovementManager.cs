using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMovementManager : MonoBehaviour
{
    public Transform _targetTransform;

    private NodeSideInfo[] _path;
    private int _targetIndex = 0;

    public List<BoardMoveBase> _availableMovements = new List<BoardMoveBase>();

    public NodeSideInfo _sittingNodeInfo;

    private BoardObject _boardObject;


    private void Awake()
    {
        _boardObject = GetComponent<BoardObject>();
    }

    private void Start()
    {
        _sittingNodeInfo = new NodeSideInfo();
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    NodeSideInfo target = new NodeSideInfo();
        //    if (BoardManager.GetNodeSideInfoAtMouse(ref target))
        //    {
        //        PathRequestManager.RequestPath(_sittingNodeInfo, target, OnPathFind);
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StopFindPath();
        //}
    }

    public void SetSittingNode(Node node, Side side)
    {
        _sittingNodeInfo = BoardManager.Instance.CurrentBoard.GetNodeInfoAt(node.X, node.Y, node.Z, side);
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
        _boardObject.UpdateNeighbourCells(info._side);

        //만약 플레이어가 죽거나 출구에 도착한다면 true를 반환한다
        if (GameStateManager.Instance.CheckMovement(_sittingNodeInfo._node))
        {
            return;
        }
        _boardObject.CheckAdjacentCells();
        foreach (var movement in _availableMovements)
        {
            movement.UpdateNodeSideInfo(_sittingNodeInfo);
        }
    }

    public void StopFindPath()
    {
        StopCoroutine("FollowPath");
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
        NodeSideInfo currentNodeInfo = _path[0];
        if (_path.Length == 1)
        {
            BoardMoveBase toUseMovement = null;
            currentNodeInfo = _path[_targetIndex];
            if (_sittingNodeInfo._side != currentNodeInfo._side)
            {
                toUseMovement = _availableMovements[2];
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
                        toUseMovement = _availableMovements[2];
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
    }
}
