using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCursor : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private LineRenderer _tipLineRenderer;

    private NodeSideInfo _selectingInfo;
    private NodeSideInfo _prevSelectingInfo;

    private bool _selectingInfoChanged = false;

    private List<NodeSideInfo> _selectedInfoList = new List<NodeSideInfo>();

    private BoardStamina _playerStamina;

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
    }

    private void OnEnable()
    {
        Debug.Log("enabled");
        LocateCursor();
        if (_selectingInfoChanged)
        {
            _prevSelectingInfo = _selectingInfo;
            _selectedInfoList.Add(_selectingInfo);
            BuildLine();
        }
    }

    private void Update()
    {
        _selectingInfoChanged = false;
        LocateCursor();

        #region Add selectingInfo to list
        if (_playerStamina.CurrentStamina > 0 &&
            _selectingInfoChanged)
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

        _prevSelectingInfo = _selectingInfo;

        #endregion

        if (Input.GetMouseButtonUp(0))
        {
            MoveCommand moveCommand = new MoveCommand(_selectedInfoList);
            GameStateManager.Instance.Player.DoCommand(moveCommand);

            CursorManager.Instance.ChangeState(CursorManager.CursorState.Select);

            _selectedInfoList.Clear();
            _lineRenderer.positionCount = 0;
        }
    }

    private void LocateCursor()
    {
        Quaternion rotation = transform.rotation;
        Vector3 worldPosition = transform.position;

        _prevSelectingInfo = _selectingInfo;

        if (BoardManager.GetNodeSideInfoAtMouse(ref _selectingInfo))
        {
            worldPosition = _selectingInfo.GetWorldPosition();
            //offset
            worldPosition += BoardManager.SideToVector3Offset(_selectingInfo._side) * 0.1f;
            rotation = BoardManager.SideToRotation(_selectingInfo._side);
            if (_selectingInfo != _prevSelectingInfo)
            {
                _selectingInfoChanged = true;
            }
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
                //float angle = Vector3.Angle(diff, Vector3.right);
                //Debug.Log(angle);
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
