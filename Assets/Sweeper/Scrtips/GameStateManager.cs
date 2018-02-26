using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;
using Utils;

public class GameStateManager : SingletonBase<GameStateManager>
{
    private BoardManager _boardManager;
    public BoardManager BoardManager { get { return _boardManager; } }

    [Header("Prefabs")]
    public GameObject _exclamationPrefab;
    public GameObject _dangerSignPrefab;

    private List<GameObject> _exclamations;

    //마우스 우클릭시 정보를 보여줄 오브젝트이다.
    private Node _selectingNode;
    public Node SelectingNode
    {
        get { return _selectingNode; }
        set { _selectingNode = value; }
    }
    private Timer _rightMouseClickTimer;    

    public Board CurrentBoard
    {
        get { return BoardManager.Instance.CurrentBoard; }
    }

    public BoardObject Player;

    public LayerMask _nodeMask;
    public LayerMask _objectMask;

    public CameraController _cameraController;


    private void Start()
    {
        _boardManager = GetComponent<BoardManager>();

        _exclamations = new List<GameObject>();

        SetupNextBoard();

        _rightMouseClickTimer = new Timer(0.5f);

    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<Events.RadarSkillEvent>(OnRadarSkillEvent);
    }

    private void OnDisable()
    {
        EventManager.Instance.AddListener<Events.RadarSkillEvent>(OnRadarSkillEvent);
    }

    public void Update()
    {
        if (_selectingNode == null)
        {
            if (Input.GetMouseButton(1) && _rightMouseClickTimer.Tick(Time.deltaTime))
            {
                NodeSideInfo info = new NodeSideInfo();
                if (BoardManager.GetNodeSideInfoAtMouse(ref info))
                {
                    _selectingNode = info._node;

                    //RadialMenu.Show(Input.mousePosition, info);
                }
            }
        }
    }

    public void SetupNextBoard()
    {
        _boardManager.BuildNewBoard();
        Player.SetSittingNode(_boardManager.CurrentBoard.GetNodeAt(_boardManager.CurrentBoard.StartCellCoord), Side.Top);
        RemoveExclamations();

        _cameraController.transform.position = new Vector3(
            _boardManager.WorldSize.x / 2.0f,
            _cameraController.transform.position.y,
            _boardManager.WorldSize.z / 2.0f);

    }

    public void RespawnPlayer()
    {
        Player.SetSittingNode(_boardManager.CurrentBoard.GetNodeAt(_boardManager.CurrentBoard.StartCellCoord), Side.Top);
    }

    public void SpawnExclamation(int x, int y, int z)
    {
        Vector3 position = BoardManager.BoardPosToWorldPos(new Vector3Int(x, y, z));
        position.y = 1.0f;
        GameObject go = Instantiate(_exclamationPrefab, position, Quaternion.identity);
        go.transform.SetParent(transform);
        _exclamations.Add(go);
    }

    public void RemoveExclamations()
    {
        if (_exclamations.Count > 0)
        {
            for (int i = _exclamations.Count - 1; i >= 0; --i)
            {
                Destroy(_exclamations[i]);
            }
            _exclamations.Clear();
        }
    }

    //만약 플레이어가 죽거나 출구에 도착한다면 true를 반환한다
    public bool CheckMovement(Node sittingNode)
    {
        bool result = false;
        if (sittingNode.IsHazard)
        {
            RespawnPlayer();
            result = true;
        }
        else if (sittingNode.Type == Node.NodeType.Exit)
        {
            SetupNextBoard();
            result = true;
        }
        return result;
    }


    public void OnRadarSkillEvent(Events.RadarSkillEvent e)
    {
        //Node[] playerAdjacentCells = Player.AdjacentCells;

        //int count = 0;
        //for (int z = 0; z < 3; ++z)
        //{
        //    for (int x = 0; x < 3; ++x)
        //    {
        //        if (x == 1 && z == 1)
        //        {
        //            continue;
        //        }
        //        Node currentNode = playerAdjacentCells[count];
        //        if (currentNode != null)
        //        {
        //            if (currentNode.IsHazard)
        //            {
        //                SpawnExclamation(currentNode.X, currentNode.Y, currentNode.Z);
        //            }
        //        }
        //        count++;
        //    }
        //}
    }
}