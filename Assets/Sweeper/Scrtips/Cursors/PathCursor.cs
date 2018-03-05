using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCursor : CursorBase
{
    private LineRenderer _lineRenderer;
    private LineRenderer _tipLineRenderer;

    private List<NodeSideInfo> _selectedInfoList = new List<NodeSideInfo>();

    private BoardStamina _playerStamina;

    CursorManager _manager;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        //_tipLineRenderer = GetComponentInChildren<LineRenderer>();
    }

    private void Start()
    {
        _playerStamina = GameStateManager.Instance.Player.GetComponent<BoardStamina>();

        _selectingInfo = BoardManager.Instance.CurrentBoard.GetNodeInfoAt(BoardManager.Instance.CurrentBoard.StartCellCoord, Side.Top);
        _prevSelectingInfo = BoardManager.Instance.CurrentBoard.GetNodeInfoAt(BoardManager.Instance.CurrentBoard.StartCellCoord, Side.Top);

        _manager = CursorManager.Instance;
    }

    public override void LocateCursor()
    {
        Quaternion rotation = transform.rotation;
        Vector3 worldPosition = transform.position;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    public override void HandleInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            MoveCommand moveCommand = new MoveCommand(_selectedInfoList);
            GameStateManager.Instance.Player.DoCommand(moveCommand);

            _selectedInfoList.Clear();
            _lineRenderer.positionCount = 0;

            CursorManager.Instance.ChangeState(CursorManager.CursorState.Select);
        }

        if (_playerStamina.CurrentStamina > 0 &&
            _manager.SelectingInfoChanged)
        {
            if (_selectedInfoList.Count >= 2 &&
                Node.IsAdjacent(_selectedInfoList[_selectedInfoList.Count - 1]._node, _selectingInfo._node))
            {
                if (Object.ReferenceEquals(_selectingInfo, _selectedInfoList[_selectedInfoList.Count - 2]))
                {
                    _selectedInfoList.RemoveAt(_selectedInfoList.Count - 1);
                }
                else
                {
                    if (_selectedInfoList.Count < _playerStamina.CurrentStamina)
                    {
                        _selectedInfoList.Add(_selectingInfo);
                    }
                }
            }
            else
            {
                if (_selectedInfoList.Count < _playerStamina.CurrentStamina)
                {
                    _selectedInfoList.Add(_selectingInfo);
                }
            }
            BuildLine();
        }
    }

    private void BuildLine()
    {
        List<Vector3> positionList = new List<Vector3>(_selectedInfoList.Count * 2);
        if (_selectedInfoList.Count > 0)
        {
            Vector3 closestEdge = Vector3.zero;

            Vector3 currentSideOffset = Vector3.zero;
            Vector3 nextSideOffset = Vector3.zero;

            NodeSideInfo current = null;
            NodeSideInfo next = null;

            if (_selectedInfoList.Count == 1)
            {
                current = _selectedInfoList[0];

                currentSideOffset = BoardManager.SideToOffset(current._side).ToVector3() * 0.1f;
                Vector3 currentMouseNodePos = GameStateManager.Instance.MouseNodeSidePosition;

                Vector3 diff = (currentMouseNodePos - currentSideOffset).normalized;
                positionList.Add(current.GetWorldPosition() + currentSideOffset);
                positionList.Add(current.GetWorldPosition() + (BoardManager.SideToVector3Offset(Side.Right) * 0.5f) + currentSideOffset);
            }
            else
            {
                for (int i = 0; i < _selectedInfoList.Count - 1; ++i)
                {
                    current = _selectedInfoList[i];
                    next = _selectedInfoList[i + 1];

                    currentSideOffset = BoardManager.SideToOffset(current._side).ToVector3() * 0.1f;
                    nextSideOffset = BoardManager.SideToOffset(next._side).ToVector3() * 0.1f;

                    closestEdge = NodeSideInfo.GetClosestEdge(current, next);
                    positionList.Add(current.GetWorldPosition() + currentSideOffset);
                    positionList.Add(closestEdge + nextSideOffset);
                }
                current = _selectedInfoList[_selectedInfoList.Count - 1];
                currentSideOffset = BoardManager.SideToOffset(current._side).ToVector3() * 0.1f;
                positionList.Add(current.GetWorldPosition() + currentSideOffset);
            }

            _lineRenderer.positionCount = positionList.Count;
            _lineRenderer.SetPositions(positionList.ToArray());
        }
    }
}
