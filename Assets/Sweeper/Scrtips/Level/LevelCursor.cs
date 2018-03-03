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
            { new Vector2(0.0f, 0.5f), new Vector2(0.0f, 1.0f), new Vector2(0.5f, 1.0f), new Vector2(0.5f, 0.5f) },
            { new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(1.0f, 0.5f) },
            { new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.0f) },
            { new Vector2(0.5f, 0.0f), new Vector2(0.5f, 0.5f), new Vector2(1.0f, 0.5f), new Vector2(1.0f, 0.0f) }
        };

        [HideInInspector]
        public NodeSideInfo _selectingInfo;
        [HideInInspector]
        public NodeSideInfo _prevSelectingInfo;

        [HideInInspector]
        public List<NodeSideInfo> _selectedNodeInfoList = new List<NodeSideInfo>();

        private bool _selectingNodeChanged = false;

        public Material _cursorMaterial;

        private Timer _rightMouseClickTimer;
        private Timer _leftMouseClockTimer;   

        private CursorState _state;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;

        private Mesh _selectMesh;
        private Mesh _lineMesh;

        private BoardStamina _playerStamina;

        private void Awake()
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            _meshRenderer.material = _cursorMaterial;
            _meshFilter = GetComponentInChildren<MeshFilter>();

            _playerStamina = GameStateManager.Instance.Player.GetComponent<BoardStamina>();
        }

        private void Start()
        {
            _rightMouseClickTimer = new Timer(0.5f);
            _leftMouseClockTimer = new Timer(0.5f);
            _state = CursorState.Select;

            _prevSelectingInfo = 
                BoardManager.Instance.CurrentBoard.GetNodeInfoAt(BoardManager.Instance.CurrentBoard.StartCellCoord, Side.Top);
            _selectingInfo = 
                BoardManager.Instance.CurrentBoard.GetNodeInfoAt(BoardManager.Instance.CurrentBoard.StartCellCoord, Side.Top);

            BuildSelecMesh();
        }

        private void Update()
        {
            //NOTE : Temp code
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    ChangeState(CursorState.Select);
            //    Debug.Log("select");
            //}
            //else if (Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    ChangeState(CursorState.Move);
            //    Debug.Log("move");
            //}
            //else if (Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    ChangeState(CursorState.Create);
            //    Debug.Log("create");
            //}

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
                _leftMouseClockTimer.Tick(Time.deltaTime) &&
                !Object.ReferenceEquals(_selectingInfo, null))
            {
                GameObject sittingObject = _selectingInfo._sittingObject;
                if (sittingObject != null &&
                    sittingObject.GetComponent<PlayerBoardObject>() != null)
                {
                    ChangeState(CursorState.Move);
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
            #region Add selectingInfo to list
            if (_playerStamina.CurrentStamina > 0 && 
                _selectingNodeChanged )
            {
                if (_selectedNodeInfoList.Count >= 2 &&
                    Node.IsAdjacent(_selectedNodeInfoList[_selectedNodeInfoList.Count - 1]._node, _selectingInfo._node))
                {
                    if (Object.ReferenceEquals(_selectingInfo, _selectedNodeInfoList[_selectedNodeInfoList.Count - 2]))
                    {
                        _selectedNodeInfoList.RemoveAt(_selectedNodeInfoList.Count - 1);
                    }
                    else
                    {
                        if (_selectedNodeInfoList.Count < _playerStamina.CurrentStamina)
                        {
                            _selectedNodeInfoList.Add(_selectingInfo);
                        }
                    }
                }
                else
                {
                    if (_selectedNodeInfoList.Count < _playerStamina.CurrentStamina)
                    {
                        _selectedNodeInfoList.Add(_selectingInfo);
                    }
                }
                BuildLineMesh();
            }
            #endregion

            if (Input.GetMouseButtonUp(0))
            {
                MoveCommand moveCommand = new MoveCommand(_selectedNodeInfoList[_selectedNodeInfoList.Count - 1]);
                GameStateManager.Instance.Player.DoCommand(moveCommand);

                ChangeState(CursorState.Select);
            }
        }

        private void LocateCursor()
        {
            Quaternion rotation = transform.rotation;
            Vector3 worldPosition = transform.position;

            _prevSelectingInfo = _selectingInfo;
            _selectingNodeChanged = false;

            if (BoardManager.GetNodeSideInfoAtMouse(ref _selectingInfo))
            {
                worldPosition = _selectingInfo.GetWorldPosition();
                //offset
                worldPosition += BoardManager.SideToVector3Offset(_selectingInfo._side) * 0.1f;
                rotation = BoardManager.SideToRotation(_selectingInfo._side);

                if (_prevSelectingInfo != _selectingInfo)
                {
                    _selectingNodeChanged = true;
                }
            }

            if (_state == CursorState.Create ||
                _state == CursorState.Select)
            {
                transform.position = worldPosition;
                transform.rotation = rotation;
            }
            else
            {
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
            }
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
            if (_selectedNodeInfoList.Count > 0)
            {
                _lineMesh = MeshBuilder.BuildQuadsFromNodeInfoList(_selectedNodeInfoList, _cursorUVs, 0.3f);
                _meshFilter.mesh = _lineMesh;
            }
        }

        private void ChangeState(CursorState state)
        {
            if (state == _state)
            {
                return;
            }

            switch (_state)
            {
                case CursorState.Create:
                    {
                    } break;
                case CursorState.Move:
                    {
                        _selectedNodeInfoList.Clear();
                    } break;
                case CursorState.Select:
                    {
                    } break;
            }

            _state = state;
            switch (state)
            {
                case CursorState.Create:
                    {
                    } break;
                case CursorState.Move:
                    {
                        _selectedNodeInfoList.Add(_selectingInfo);
                        BuildLineMesh();
                    } break;
                case CursorState.Select:
                    {
                        _meshFilter.mesh = _selectMesh;
                    } break;
            }
        }

    }

}
