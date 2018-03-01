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

    public BoardObject Player;

    public CameraController _cameraController;

    private void Start()
    {
        _boardManager = BoardManager.Instance;

        _exclamations = new List<GameObject>();

        SetupNextBoard();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<Events.RadarSkillEvent>(OnRadarSkillEvent);
    }

    private void OnDisable()
    {
        EventManager.Instance.AddListener<Events.RadarSkillEvent>(OnRadarSkillEvent);
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

    public void SpawnExclamation(Transform playerTransform)
    {
        GameObject go = Instantiate(_exclamationPrefab, playerTransform.position + Vector3.up, Quaternion.identity);
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
    public bool CheckMovement(NodeSideInfo sittingNodeInfo)
    {
        bool result = false;
        if (sittingNodeInfo.IsHazard)
        {
            RespawnPlayer();
            result = true;
        }
        else if (sittingNodeInfo._node.Type == Node.NodeType.Exit)
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