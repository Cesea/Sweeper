using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

using Utils;
using Level;

public class LevelCreateCursor : MonoBehaviour
{
    public static Vector2[,] _cursorUVs =
    {
            { new Vector2(0.0f, 0.5f), new Vector2(0.0f, 1.0f), new Vector2(0.5f, 1.0f), new Vector2(0.5f, 0.5f) },
            { new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(1.0f, 0.5f) },
            { new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.0f) },
            { new Vector2(0.5f, 0.0f), new Vector2(0.5f, 0.5f), new Vector2(1.0f, 0.5f), new Vector2(1.0f, 0.0f) }
        };

    [HideInInspector]
    public NodeSideInfo _selectingInfo;

    public Material _cursorMaterial;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;

    private Mesh _selectMesh;

    private BoardStamina _playerStamina;

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _meshRenderer.material = _cursorMaterial;
        _meshFilter = GetComponentInChildren<MeshFilter>();
    }

    private void Start()
    {
        _selectingInfo =
            BoardManager.Instance.CurrentBoard.GetNodeInfoAt(BoardManager.Instance.CurrentBoard.StartCellCoord, Side.Top);
        BuildSelecMesh();
    }

    private void Update()
    {
        LocateCursor();

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
        {
            LevelCreator.Instance.DestroyObjectAtNode(_selectingInfo);
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            LevelCreator.Instance.InstallObjectAtNode(_selectingInfo, LevelCreator.Instance.SelectingIndex);
            return;
        }
    }

    //private void SelectUpdate()
    //{
    //    if (Input.GetMouseButton(0) &&
    //        _leftMouseClockTimer.Tick(Time.deltaTime) &&
    //        !Object.ReferenceEquals(_selectingInfo, null))
    //    {
    //        GameObject sittingObject = _selectingInfo.SittingObject;
    //        if (sittingObject != null &&
    //            sittingObject.GetComponent<PlayerBoardObject>() != null)
    //        {
    //            ChangeState(CursorState.Move);
    //        }
    //    }

    //    if (Input.GetMouseButton(1) && _rightMouseClickTimer.Tick(Time.deltaTime))
    //    {
    //        if (!Object.ReferenceEquals(_selectingInfo, null) &&
    //            !RadialMenu._opened &&
    //            BoardManager.GetNodeSideInfoAtMouse(ref _selectingInfo))
    //        {
    //            RadialMenu.Show(_selectingInfo);
    //            _rightMouseClickTimer.Reset();
    //        }
    //    }
    //    if (RadialMenu._opened &&
    //        Input.GetMouseButtonUp(1))
    //    {
    //        RadialMenu.Shut();
    //    }
    //}

    private void LocateCursor()
    {
        Quaternion rotation = transform.rotation;
        Vector3 worldPosition = transform.position;

        if (BoardManager.GetNodeSideInfoAtMouse(ref _selectingInfo))
        {
            worldPosition = _selectingInfo.GetWorldPosition();
            //offset
            worldPosition += BoardManager.SideToVector3Offset(_selectingInfo._side) * 0.1f;
            rotation = BoardManager.SideToRotation(_selectingInfo._side);
        }

        transform.position = worldPosition;
        transform.rotation = rotation;
    }

    private void BuildSelecMesh()
    {
        _selectMesh = MeshBuilder.BuildQuad(Side.Top,
                                            BoardManager.Instance.NodeRadius,
                                            new Vector3(0.0f, -BoardManager.Instance.NodeRadius, 0.0f));
        Vector2[] uvs = { _cursorUVs[3, 0], _cursorUVs[3, 1], _cursorUVs[3, 2], _cursorUVs[3, 3] };
        _selectMesh.uv = uvs;

        _meshFilter.mesh = _selectMesh;
    }

}
