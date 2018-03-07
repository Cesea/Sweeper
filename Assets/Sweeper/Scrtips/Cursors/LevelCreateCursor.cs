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
    private Material _lineMaterial;
    [SerializeField]
    private Material _invalidLineMaterial;

    private Timer _leftClickTimer;
    private Timer _rightClickTimer;

    private CursorManager _manager;

    private LevelObject _previewObject;

    private int _selectingIndex = 0;
    public int SelectingIndex { get { return _selectingIndex; } }
    private int _prevIndex = 0;

    private Vector3 _createOffset;
    private Quaternion _createRotation;

    private bool _positioning =false;
    private Vector3 _horizontalDeltaAxis = Vector3.zero;
    private Vector3 _verticalDeltaAxis = Vector3.zero;

    private Vector3 _prevMousePosition = Vector3.zero;
    private Vector3 _mousePosition = Vector3.zero;

    private List<GameObject> _installPrefabList;
    public List<GameObject> InstallPrefabList { get { return _installPrefabList; } }

    public override void UpdateSelectingInfos(NodeSideInfo selecting, NodeSideInfo prev)
    {
        if (!_positioning)
        {
            _selectingInfo = selecting;
            _prevSelectingInfo = prev;
        }
    }

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        _leftClickTimer = new Timer(0.5f);
        _rightClickTimer = new Timer(0.5f);
    }

    private void Start ()
    {
        _lineRenderer.loop = true;

        _lineRenderer.material = _lineMaterial;

        _lineRenderer.positionCount = 4;
        Vector3[] positions = new Vector3[4];
        positions[0] = new Vector3(-0.5f, 0.0f, -0.5f);
        positions[1] = new Vector3(0.5f, 0.0f, -0.5f);
        positions[2] = new Vector3(0.5f, 0.0f, 0.5f);
        positions[3] = new Vector3(-0.5f, 0.0f, 0.5f);
        _lineRenderer.SetPositions(positions);

        _selectingInfo = BoardManager.Instance.CurrentBoard.GetNodeInfoAt(BoardManager.Instance.CurrentBoard.StartCellCoord, Side.Top);


        EventManager.Instance.AddListener<Events.LevelCreatorMenuEvent>(OnLevelCreatorMenuEvent);
	}

    private void OnEnable()
    {
        _manager = CursorManager.Instance;
        _installPrefabList = LevelCreator.Instance.InstallPrefabList;
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener<Events.LevelCreatorMenuEvent>(OnLevelCreatorMenuEvent);
    }

    public override void LocateCursor()
    {
        if (_selectingIndex != _prevIndex)
        {
            RecreateObjectToInstall();
        }
        _prevIndex = _selectingIndex;
        if (_positioning)
        {
            if (_previewObject != null)
            {
                _previewObject.transform.position = _selectingInfo.GetWorldPosition() + _createOffset;
                //_previewObject.transform.Rotate(_createRotation);
            }
        }
        else
        {
            MakeObjectToInstallFollowCursor();
        }
        //locate cursor
        if (!_positioning)
        {
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
    }

    public override void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        bool leftMouseClick = Input.GetMouseButtonDown(0);
        bool leftMouseUp = Input.GetMouseButtonUp(0);

        bool rightMouseClick = Input.GetMouseButtonDown(1);
        bool shiftDown = Input.GetKey(KeyCode.LeftShift);

        _prevMousePosition = _mousePosition;
        _mousePosition = Input.mousePosition;

        if (_manager.SelectionValid)
        {
            if (leftMouseClick && shiftDown &&
                !Object.ReferenceEquals(_selectingInfo, null))
            {
                LevelCreator.Instance.DestroyObjectAtNode(_selectingInfo);
                return;
            }

            if (leftMouseClick &&
                !Object.ReferenceEquals(_selectingInfo, null))
            {
                switch (_selectingInfo._side)
                {
                    case Side.Back:
                    case Side.Front:
                        {
                            _horizontalDeltaAxis = new Vector3(1.0f, 0.0f, 0.0f);
                            _verticalDeltaAxis = new Vector3(0.0f, 1.0f, 0.0f);
                        } break;
                    case Side.Left:
                    case Side.Right:
                        {
                            _horizontalDeltaAxis = new Vector3(0.0f, 0.0f, 1.0f);
                            _verticalDeltaAxis = new Vector3(0.0f, 1.0f, 0.0f);
                        } break;
                    case Side.Top:
                    case Side.Bottom:
                        {
                            _horizontalDeltaAxis = new Vector3(1.0f, 0.0f, 0.0f);
                            _verticalDeltaAxis = new Vector3(0.0f, 0.0f, 1.0f);
                        } break;
                }
                _positioning = true;
                return;
            }
        }

        if (_positioning)
        {
            if (rightMouseClick)
            {
                _createOffset = Vector3.zero;
                _createRotation = Quaternion.identity;
            }

            if (leftMouseUp && _manager.SelectionValid)
            {
                _positioning = false;
                LevelCreator.Instance.InstallObjectAtNode(_selectingInfo, _selectingIndex, 
                    _createOffset, BoardManager.SideToRotation(_selectingInfo._side) *_createRotation);
                _createOffset = Vector3.zero;
                _createRotation = Quaternion.identity;
                return;
            }

            float screenToWorldScale = 0.01f;
            Vector3 deltaMousePosition = _mousePosition - _prevMousePosition;
            if (deltaMousePosition.sqrMagnitude > 3.0f)
            {
                Vector3 moveDelta = (_horizontalDeltaAxis * deltaMousePosition.x * screenToWorldScale) +
                    (_verticalDeltaAxis * deltaMousePosition.y * screenToWorldScale);

                _createOffset += moveDelta;
                _createOffset = _createOffset.ClampXYZ(-0.5f, 0.5f);
            }
        }
    }

    public void OnLevelCreatorMenuEvent(Events.LevelCreatorMenuEvent e)
    {
        if (e.Opened)
        {
            RecreateObjectToInstall();
        }
        else
        {
            Destroy(_previewObject);
        }
    }

    #region  Utils
    private void RecreateObjectToInstall()
    {
        if (_previewObject != null)
        {
            Destroy(_previewObject);
            _previewObject = null;
        }

        if (_previewObject == null)
        {
            _previewObject = Level.LevelCreator.Instance.CreateObjectAtNode(_selectingInfo, _installPrefabList[_selectingIndex],
                Vector3.zero, Quaternion.identity).GetComponent<LevelObject>();
        }
    }

    private void MakeObjectToInstallFollowCursor()
    {
        if (_previewObject != null)
        {
            _previewObject.transform.position = transform.position;
            if (_previewObject.CanInstallSide)
            {
                _previewObject.transform.rotation = BoardManager.SideToRotation(_selectingInfo._side);
                _lineRenderer.material = _lineMaterial;
            }
            else
            {
                if (BoardManager.IsSideHorizontal(_selectingInfo._side))
                {
                    _lineRenderer.material = _invalidLineMaterial;
                }
                else
                {
                    _lineRenderer.material = _lineMaterial;
                }
            }
        }
    }

    public void SetSelectingIndex(int index)
    {
        if (index > _installPrefabList.Count)
        {
            return;
        }
        _selectingIndex = index;
        RecreateObjectToInstall();
    }
    #endregion

}

