using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class GameStateManager : SingletonBase<GameStateManager>
{
    private BoardManager _boardManager;
    public BoardManager BoardManager { get { return _boardManager; } }

    public GameObject _exclamationPrefab;

    private List<GameObject> _exclamations;
    private List<GameObject> _dangerSigns;


    public Board CurrentBoard
    {
        get { return BoardManager.Instance.CurrentBoard; }
    }

    public BoardMover Player;

    private void Start()
    {
        _boardManager = GetComponent<BoardManager>();

        _boardManager.BuildNewBoard(10, 10);
        Player.SetPosition(_boardManager.CurrentBoard.StartCellCoord);

        _exclamations = new List<GameObject>();
        _dangerSigns = new List<GameObject>();
    }

    public void SetupNextBoard()
    {
        _boardManager.BuildNewBoard(10, 10);
        Player.SetPosition(_boardManager.CurrentBoard.StartCellCoord);
        RemoveExclamations();
    }

    public void RespawnPlayer()
    {
        Player.SetPosition(_boardManager.CurrentBoard.StartCellCoord);
    }

    public void SpawnExclamation(int x, int z)
    {
        GameObject go = Instantiate(_exclamationPrefab, new Vector3(x, 0, z), Quaternion.identity);
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
}
