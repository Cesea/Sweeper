using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class GameStateManager : SingletonBase<GameStateManager>
{
    private BoardManager _boardManager;
    public BoardManager BoardManager { get { return _boardManager; } }

    private void Start()
    {
        _boardManager = GetComponent<BoardManager>();
    }

    public void LoadNextLevel()
    {
        _boardManager.LoadNextLevel();
    }

    public void CheckMovement(int x, int z)
    {
        Cell.CellType currentCell = _boardManager.CurrentBoard.GetTypeAt( x, z);
        switch (currentCell)
        {
            case Cell.CellType.Empty:
                {
                } break;
            case Cell.CellType.Mine:
                {
                    Debug.Log("Mine");
                } break;
            case Cell.CellType.Start:
                {
                } break;
            case Cell.CellType.Exit:
                {
                    LoadNextLevel(); 
                } break;
        }
    }
}
