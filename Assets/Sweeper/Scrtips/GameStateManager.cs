using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameStateManager>();
                if (_instance == null)
                {
                    Debug.Log("GamestateManager doesn't exist");
                }
            }
            return _instance;
        }
    }

    private Board _currentBoard;
    public Board CurrentBoard { get { return _currentBoard; } }

    private void Awake()
    {
        _instance = this;
        
    }

    private void Start()
    {

        _currentBoard = FindObjectOfType<Board>();
    }

    public void LoadNewBoard()
    {
        if (_currentBoard != null)
        {

        }
    }
}
