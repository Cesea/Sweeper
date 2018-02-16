using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class GameStateManager : SingletonBase<GameStateManager>
{
    private Board _currentBoard;
    public Board CurrentBoard { get { return _currentBoard; } }

    private void Start()
    {

        _currentBoard = FindObjectOfType<Board>();
    }

    public void LoadNextLevel()
    {
        LevelManager.Instance.LoadNextLevel();
    }

    public void CheckMovement(int x, int z)
    {
        Cell.CellType currentCell = GameStateManager.Instance.CurrentBoard.GetTypeAt( x, z);
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
