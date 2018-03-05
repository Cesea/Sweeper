﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

using Utils;
using Level;

public class LevelCreateCursor : MonoBehaviour
{
   private LineRenderer _lineRenderer;
    private NodeSideInfo _selectingInfo;

    [SerializeField]
    private Material _material;

    private Timer _leftClickTimer;
    private Timer _rightClickTimer;

    private bool _cursorInBoard;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        _leftClickTimer = new Timer(0.5f);
        _rightClickTimer = new Timer(0.5f);
    }

    private void Start ()
    {
        _lineRenderer.loop = true;

        _lineRenderer.material = _material;

        _lineRenderer.positionCount = 4;
        Vector3[] positions = new Vector3[4];
        positions[0] = new Vector3(-0.5f, 0.0f, -0.5f);
        positions[1] = new Vector3(0.5f, 0.0f, -0.5f);
        positions[2] = new Vector3(0.5f, 0.0f, 0.5f);
        positions[3] = new Vector3(-0.5f, 0.0f, 0.5f);
        _lineRenderer.SetPositions(positions);

        _selectingInfo = BoardManager.Instance.CurrentBoard.GetNodeInfoAt(BoardManager.Instance.CurrentBoard.StartCellCoord, Side.Top);
	}
	
	private void Update ()
    {
        //locate cursor
        Quaternion rotation = transform.rotation;
        Vector3 worldPosition = transform.position;

        _cursorInBoard = BoardManager.GetNodeSideInfoAtMouse(ref _selectingInfo);
        if(_cursorInBoard)
        {
            worldPosition = _selectingInfo.GetWorldPosition();
            //offset
            worldPosition += BoardManager.SideToVector3Offset(_selectingInfo._side) * 0.1f;
            rotation = BoardManager.SideToRotation(_selectingInfo._side);
        }
        transform.position = worldPosition;
        transform.rotation = rotation;

        HandleInput();
	}

    private void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (_cursorInBoard)
        {
            bool leftMouseClick = Input.GetMouseButtonDown(0);
            bool shiftDown = Input.GetKey(KeyCode.LeftShift);

            if (leftMouseClick && shiftDown &&
                !Object.ReferenceEquals(_selectingInfo, null))
            {
                LevelCreator.Instance.DestroyObjectAtNode(_selectingInfo);
                return;
            }

            if (leftMouseClick &&
                !Object.ReferenceEquals(_selectingInfo, null))
            {
                LevelCreator.Instance.InstallObjectAtNode(_selectingInfo);
                return;
            }
        }
    }
}

