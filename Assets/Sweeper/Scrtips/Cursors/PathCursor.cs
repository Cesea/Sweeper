using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCursor : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    private NodeSideInfo _selectingInfo;
    private NodeSideInfo _prevSelectingInfo;

    private bool _selecingInfoChanged = false;

    private List<NodeSideInfo> _selectedInfoList = new List<NodeSideInfo>();

    private BoardStamina _playerStamina;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        BoardManager.Instance.CurrentBoard.GetNodeInfoAt(BoardManager.Instance.CurrentBoard.StartCellCoord, Side.Top);
        BoardManager.Instance.CurrentBoard.GetNodeInfoAt(BoardManager.Instance.CurrentBoard.StartCellCoord, Side.Top);

        _playerStamina = GameStateManager.Instance.Player.GetComponent<BoardStamina>();
    }

    private void Update()
    {
        LocateCursor();

        #region Add selectingInfo to list
        if (_playerStamina.CurrentStamina > 0 &&
            _selecingInfoChanged)
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
        #endregion

        if (Input.GetMouseButtonUp(0))
        {
            MoveCommand moveCommand = new MoveCommand(_selectedInfoList[_selectedInfoList.Count - 1]);

            GameStateManager.Instance.Player.DoCommand(moveCommand);
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
                _selecingInfoChanged = true;
            }
        }
    }

    private void BuildLine()
    {
        if (_selectedInfoList.Count > 0)
        {
            //_lineMesh = MeshBuilder.BuildQuadsFromNodeInfoList(_selectedNodeInfoList, _cursorUVs, 0.3f);
            //_meshFilter.mesh = _lineMesh;
        }
    }

    //private void BuildLineMesh()
    //{
    //    if (_selectedNodeInfoList.Count > 0)
    //    {
    //        _lineMesh = MeshBuilder.BuildQuadsFromNodeInfoList(_selectedNodeInfoList, _cursorUVs, 0.3f);
    //        _meshFilter.mesh = _lineMesh;
    //    }
    //}


}
