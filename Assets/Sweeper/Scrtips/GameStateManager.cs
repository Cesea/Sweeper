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
    private GameObject _selectingObject;
    public GameObject SelectingObject
    {
        get { return _selectingObject; }
        set { _selectingObject = value; }
    }
    private Timer _rightMouseClickTimer;    

    public Board CurrentBoard
    {
        get { return BoardManager.Instance.CurrentBoard; }
    }

    public BoardObject Player;

    public LayerMask _nodeMask;
    public LayerMask _objectMask;

    public BoxCollider MouseCollisionBox;

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
        if (_selectingObject == null)
        {
            if (Input.GetMouseButton(1) && _rightMouseClickTimer.Tick(Time.deltaTime))
            {
                _selectingObject = GetObjectUnderneathMouse();
                if (_selectingObject != null)
                {
                    RadialMenu.Show(Input.mousePosition, _selectingObject);
                }
            }
        }
    }

    public void SetupNextBoard()
    {
        _boardManager.BuildNewBoard();
        Player.SetPosition(_boardManager.CurrentBoard.StartCellCoord);
        RemoveExclamations();

        CreateMouseCollisionBox();

    }

    private void CreateMouseCollisionBox()
    {
        if (MouseCollisionBox == null)
        {
            MouseCollisionBox = gameObject.AddComponent<BoxCollider>();
        }
        MouseCollisionBox.size = new Vector3(_boardManager.WorldSize.x, 0.2f, _boardManager.WorldSize.y);
        MouseCollisionBox.center = new Vector3(_boardManager.WorldSize.x * 0.5f, 0.1f, _boardManager.WorldSize.y * 0.5f);
        MouseCollisionBox.isTrigger = true;
    }

    public void RespawnPlayer()
    {
        Player.SetPosition(_boardManager.CurrentBoard.StartCellCoord);
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
    public bool CheckMovement(Vector3Int pos)
    {
        return CheckMovement(pos.x, pos.y, pos.z);
    }
    public bool CheckMovement(int x, int y, int z)
    {
        bool result = false;
        Node currentNode = _boardManager.CurrentBoard.GetNodeAt(x, y, z);
        if (currentNode.IsHazard)
        {
            RespawnPlayer();
            result = true;
        }
        //else if (currentNode.IsExit)
        //{
        //    SetupNextBoard();
        //    result = true;
        //}
        return result;
    }

    //일단 오브젝트가 있는 땅이면 못간다....
    public static GameObject GetObjectUnderneathMouse()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        {
            RaycastHit hitInfo;
            if (Instance.MouseCollisionBox.Raycast(camRay, out hitInfo, 1000.0f))
            {
                Node node = BoardManager.Instance.CurrentBoard.GetNodeAt(hitInfo.point);
                if (node != null)
                {
                    //return BoardManager.Instance.GetObjectAt(node.X, node.Z);
                }
            }
        }
        return null;
    }

    public void OnRadarSkillEvent(Events.RadarSkillEvent e)
    {
        Node[] playerAdjacentCells = Player.AdjacentCells;

        int count = 0;
        for (int z = 0; z < 3; ++z)
        {
            for (int x = 0; x < 3; ++x)
            {
                if (x == 1 && z == 1)
                {
                    continue;
                }
                Node currentNode = playerAdjacentCells[count];
                if (currentNode != null)
                {
                    if (currentNode.IsHazard)
                    {
                        SpawnExclamation(currentNode.X, currentNode.Y, currentNode.Z);
                    }
                }
                count++;
            }
        }
    }
}