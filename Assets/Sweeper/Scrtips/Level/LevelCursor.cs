using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

namespace Level
{
    public class LevelCursor : MonoBehaviour
    {
        public enum CursorState
        {
            Select,
            Move,
            Create
        }

        public static Vector2[,] _cursorUVs =
        { 
            { new Vector2(0.0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.0f, 1.0f), new Vector2(0.5f,1.0f) },
            { new Vector2(0.5f, 0.5f), new Vector2(1.0f, 0.5f), new Vector2(0.5f, 1.0f), new Vector2(1.0f,1.0f) },
            { new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.0f), new Vector2(0.0f, 0.5f), new Vector2(0.5f,0.5f) },
            { new Vector2(0.5f, 0.0f), new Vector2(0.5f, 0.5f), new Vector2(1.0f, 0.5f), new Vector2(1.0f,0.0f) }
        };

        [HideInInspector]
        public NodeSideInfo _selectingInfo;
        [HideInInspector]
        public NodeSideInfo _prevSelectingInfo;

        [HideInInspector]
        public List<NodeSideInfo> _nodeInfoList = new List<NodeSideInfo>();

        private bool _locationChanged = false;

        public Material _cursorMaterial;

        private Timer _rightMouseClickTimer;

        private CursorState _state;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;

        private Mesh _selectMesh;
        private Mesh _lineMesh;

        private void Awake()
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            _meshRenderer.material = _cursorMaterial;
            _meshFilter = GetComponentInChildren<MeshFilter>();
        }

        private void Start()
        {
            _rightMouseClickTimer = new Timer(0.5f);
            _state = CursorState.Select;

            _prevSelectingInfo = 
                BoardManager.Instance.CurrentBoard.GetNodeInfoAt(BoardManager.Instance.CurrentBoard.StartCellCoord, Side.Top);
            _selectingInfo = 
                BoardManager.Instance.CurrentBoard.GetNodeInfoAt(BoardManager.Instance.CurrentBoard.StartCellCoord, Side.Top);

            BuildSelecMesh();
        }

        private void Update()
        {
            LocateCursor();
            switch (_state)
            {
                case CursorState.Select:
                    {
                        SelectUpdate();
                    }
                    break;
                case CursorState.Move:
                    {
                        MoveUpdate();
                    }
                    break;
                case CursorState.Create:
                    {
                        CreateUpdate();
                    }
                    break;
            }
        }

        private void SelectUpdate()
        {
            if (Input.GetMouseButton(0) &&
                Object.ReferenceEquals(_selectingInfo, null))
            {
                GameObject sittingObject = _selectingInfo._sittingObject;
                if (sittingObject != null)
                {
                    //GameStateManager.Instance._selectingObject  = sittingObject;
                }
            }

            if (Input.GetMouseButton(1) && _rightMouseClickTimer.Tick(Time.deltaTime))
            {
                if (!Object.ReferenceEquals(_selectingInfo, null) && 
                    !RadialMenu._opened &&
                    BoardManager.GetNodeSideInfoAtMouse(ref _selectingInfo))
                {
                    RadialMenu.Show(_selectingInfo);
                    _rightMouseClickTimer.Reset();
                }
            }

            if (RadialMenu._opened &&
                Input.GetMouseButtonUp(1))
            {
                RadialMenu.Shut();
            }
        }

        private void CreateUpdate()
        {
        }

        private void MoveUpdate()
        {
        }

        private void LocateCursor()
        {
            Quaternion rotation = transform.rotation;
            Vector3 worldPosition = transform.position;

            _prevSelectingInfo = _selectingInfo;
            _locationChanged = false;

            if (BoardManager.GetNodeSideInfoAtMouse(ref _selectingInfo))
            {
                worldPosition = _selectingInfo.GetWorldPosition();
                worldPosition += BoardManager.SideToVector3Offset(_selectingInfo._side) * 0.1f;
                rotation = BoardManager.SideToRotation(_selectingInfo._side);

                if (_prevSelectingInfo != _selectingInfo)
                {
                    _locationChanged = true;
                }
            }
            transform.position = worldPosition;
            transform.rotation = rotation;
        }

        private void BuildSelecMesh()
        {
            _selectMesh = MeshBuilder.BuildQuad(Side.Top, 
                                                BoardManager.Instance.NodeRadius, 
                                                new Vector3(0.0f, -BoardManager.Instance.NodeRadius, 0.0f));
            Vector2[] uvs = {_cursorUVs[3, 0], _cursorUVs[3, 1], _cursorUVs[3, 2], _cursorUVs[3, 3] };
            _selectMesh.uv = uvs;

            _meshFilter.mesh = _selectMesh;
        }

        private void BuildLineMesh()
        {
            if (_nodeInfoList.Count > 0)
            {
                //TODO : 
                //_selectMesh = MeshBuilder.BuildQuadsFromNodeInfoList(_nodeInfoList, _cursorUVs);
            }
        }

        private void ChangeCursorMesh(Mesh mesh)
        {
            _meshFilter.mesh = mesh;
        }

    }

}
