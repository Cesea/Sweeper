using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

using Utils;
using Level;

public class LevelCreateCursor : CursorBase
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
        //locate cursor
        Quaternion rotation = transform.rotation;
        Vector3 worldPosition = transform.position;

        if (CursorManager.Instance.SelectionValid)
        {
            worldPosition = _selectingInfo.GetWorldPosition();
            worldPosition += BoardManager.SideToVector3Offset(_selectingInfo._side) * 0.1f;
            rotation = BoardManager.SideToRotation(_selectingInfo._side);
        }
        transform.position = worldPosition;
        transform.rotation = rotation;
    }

    public override void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (_manager.SelectionValid)
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

