﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

public class SelectCursor : CursorBase
{
    private LineRenderer _lineRenderer;

    [SerializeField]
    private Material _material;

    private Timer _leftClickTimer;
    private Timer _rightClickTimer;

    private CursorManager _manager;

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

        _manager = CursorManager.Instance;
	}
	
    public override void LocateCursor()
    {
        if (_manager.SelectionValid)
        {
            Vector3 worldPosition = _selectingInfo.GetWorldPosition();
            worldPosition += BoardManager.SideToVector3Offset(_selectingInfo._side) * 0.1f;
            Quaternion rotation = BoardManager.SideToRotation(_selectingInfo._side);
            transform.position = worldPosition;
            transform.rotation = rotation;
        }
    }

    public override void HandleInput()
    {
        bool leftClicked = Input.GetMouseButton(0);
        bool rightClicked = Input.GetMouseButton(1);

        bool rightUp = Input.GetMouseButtonUp(1);

        if (leftClicked &&
            _leftClickTimer.Tick(Time.deltaTime) &&
            _selectingInfo.SittingObject != null &&
            _selectingInfo.SittingObject.GetComponent<PlayerBoardObject>() != null)
        {
            _leftClickTimer.Reset();
            CursorManager.Instance.ChangeState(CursorManager.CursorState.Move);
        }

        if (rightClicked &&
            _rightClickTimer.Tick(Time.deltaTime) &&
            !Object.ReferenceEquals(_selectingInfo, null) &&
            !RadialMenu.Opened)
        {
            _rightClickTimer.Reset();
            RadialMenu.Show(_selectingInfo);
            _lineRenderer.enabled = false;
        }

        if (rightUp &&
            RadialMenu.Opened)
        {
            _lineRenderer.enabled = true;
            RadialMenu.Shut();
        }
    }
}
