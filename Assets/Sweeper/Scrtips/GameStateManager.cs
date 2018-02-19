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
    private List<GameObject> _dangerSigns;

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

    public LayerMask _cellMask;
    public LayerMask _objectMask;

    private void Start()
    {
        _boardManager = GetComponent<BoardManager>();

        _boardManager.BuildNewBoard(10, 10);
        Player.SetPosition(_boardManager.CurrentBoard.StartCellCoord);

        _exclamations = new List<GameObject>();
        _dangerSigns = new List<GameObject>();

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
        _boardManager.BuildNewBoard(10, 10);
        Player.SetPosition(_boardManager.CurrentBoard.StartCellCoord);
        RemoveExclamations();
        RemoveDangerSigns();
    }

    public void RespawnPlayer()
    {
        Player.SetPosition(_boardManager.CurrentBoard.StartCellCoord);
    }

    public void SpawnExclamation(int x, int z)
    {
        GameObject go = Instantiate(_exclamationPrefab, new Vector3(x, 1, z), Quaternion.identity);
        go.transform.SetParent(transform);
        _exclamations.Add(go);
    }

    public void SpaenDangerSign(int x, int z)
    {
        GameObject go = Instantiate(_dangerSignPrefab, new Vector3(x, 1, z), Quaternion.identity);
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

    public void RemoveDangerSigns()
    {
        if (_dangerSigns.Count > 0)
        {
            for (int i = _dangerSigns.Count - 1; i >= 0; --i)
            {
                Destroy(_dangerSigns[i]);
            }
            _dangerSigns.Clear();
        }
    }

    //만약 플레이어가 죽거나 출구에 도착한다면 true를 반환한다
    public bool CheckMovement(int x, int z)
    {
        bool result = false;
        Cell.CellType currentCell = _boardManager.CurrentBoard.GetTypeAt(x, z);
        switch (currentCell)
        {
            case Cell.CellType.Empty:
                {
                } break;
            case Cell.CellType.Mine:
                {
                    RespawnPlayer();
                    result = true;
                } break;
            case Cell.CellType.Start:
                {
                } break;
            case Cell.CellType.Exit:
                {
                    SetupNextBoard();
                    result = true;
                } break;
        }
        return result;
    }

    //일단 오브젝트가 있는 땅이면 못간다....
    private GameObject GetObjectUnderneathMouse()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(camRay, out hitInfo, _objectMask))
            {
                if (hitInfo.collider.gameObject.GetComponent<Interactable>() != null)
                {
                    return hitInfo.collider.gameObject;
                }
            }
        }
        //{
        //    RaycastHit hitInfo;
        //    if (Physics.Raycast(camRay, out hitInfo, _cellMask))
        //    {
        //        if (hitInfo.collider.gameObject.GetComponent<Interactable>() != null)
        //        {
        //            return hitInfo.collider.gameObject;
        //        }
        //    }
        //}
        return null;
    }

    public void OnRadarSkillEvent(Events.RadarSkillEvent e)
    {
        Cell[] playerAdjacentCells = Player.AdjacentCells;

        int count = 0;
        for (int z = 0; z < 3; ++z)
        {
            for (int x = 0; x < 3; ++x)
            {
                if (x == 1 && z == 1)
                {
                    continue;
                }
                Cell currentCell = playerAdjacentCells[count];
                if (currentCell != null)
                {
                    if (currentCell.Type == Cell.CellType.Mine)
                    {
                        SpawnExclamation(currentCell.X, currentCell.Z);
                    }
                }
                count++;
            }
        }
    }
}