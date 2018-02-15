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

    public void LoadNewBoard()
    {
        if (_currentBoard != null)
        {
            LevelManager.Instance.LoadNextLevel();
        }
    }
}
