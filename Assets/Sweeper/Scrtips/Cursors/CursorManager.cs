using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class CursorManager : SingletonBase<CursorManager>
{
    public enum CursorState
    {
        Select,
        Move,
        Create,
        Count
    }

    [SerializeField]
    private LevelCreateCursor _createCursor;
    [SerializeField]
    private PathCursor _pathCursor;
    [SerializeField]
    private SelectCursor _selectCursor;

    private CursorBase _currentCursor = null;


    private NodeSideInfo _selectingInfo;
    public NodeSideInfo SelectingInfo { get { return _selectingInfo; } set { _selectingInfo = value; } }

    private NodeSideInfo _prevSelectingInfo;
    public NodeSideInfo PrevSelectingInfo { get { return _prevSelectingInfo; } set { _prevSelectingInfo = value; } }

    private bool _selectingInfoChanged = false;
    public bool SelectingInfoChanged { get { return _selectingInfoChanged; } }

    private bool _selectionValid = false;
    public bool SelectionValid { get { return _selectionValid; } }

    private CursorState _currentState = CursorState.Count;

    private void Start()
    {
        _createCursor.gameObject.SetActive(false);
        _pathCursor.gameObject.SetActive(false);
        _selectCursor.gameObject.SetActive(false);

        ChangeState(CursorState.Select);
    }

    private void Update()
    {
        _selectingInfoChanged = false;
        _prevSelectingInfo = _selectingInfo;

        _selectionValid = BoardManager.GetNodeSideInfoAtMouse(ref _selectingInfo);
        if(_selectionValid)
        {
            if (_selectingInfo != _prevSelectingInfo)
            {
                _selectingInfoChanged = true;
            }
        }

        _currentCursor.UpdateSelectingInfos(_selectingInfo, _prevSelectingInfo);
        _currentCursor.LocateCursor();
        _currentCursor.HandleInput();
    }

    public void ChangeState(CursorState newState)
    {
        if (_currentState == newState)
        {
            return;
        }
        switch (_currentState)
        {
            case CursorState.Select: { _selectCursor.gameObject.SetActive(false); } break;
            case CursorState.Move: { _pathCursor.gameObject.SetActive(false); } break;
            case CursorState.Create: { _createCursor.gameObject.SetActive(false); } break;
        }

        _currentState = newState;
        switch (newState)
        {
            case CursorState.Select:
                {
                    _selectCursor.gameObject.SetActive(true);
                    _currentCursor = _selectCursor;
                }
                break;
            case CursorState.Move:
                {
                    _pathCursor.gameObject.SetActive(true);
                    _currentCursor = _pathCursor;
                }
                break;
            case CursorState.Create:
                {
                    _createCursor.gameObject.SetActive(true);
                    _currentCursor = _createCursor;
                }
                break;
        }
    }

}
