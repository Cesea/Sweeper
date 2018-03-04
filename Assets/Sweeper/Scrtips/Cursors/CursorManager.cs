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
            case CursorState.Select: { _selectCursor.gameObject.SetActive(true); } break;
            case CursorState.Move: { _pathCursor.gameObject.SetActive(true); } break;
            case CursorState.Create: { _createCursor.gameObject.SetActive(true); } break;
        }
    }

}
