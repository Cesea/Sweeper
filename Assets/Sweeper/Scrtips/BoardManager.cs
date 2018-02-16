using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class BoardManager : SingletonBase<BoardManager>
{
    [Header ("Cube Prefabs")]
    [SerializeField]
    GameObject _emptyPrefab;
    [SerializeField]
    GameObject _minePrefab;
    [SerializeField]
    GameObject _startPrefab;
    [SerializeField]
    GameObject _exitPrefab;


    [Header("Board Size")]
    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;

    private Board _currentBoard;
    public Board CurrentBoard { get { return _currentBoard; } }

    private List<GameObject> _instantiatedCubes = new List<GameObject>();

    private void Start()
    {
        BuildBoard(_width, _height);
        BuildObjects();
    }

    public void BuildBoard(int width, int height)
    {
        _currentBoard = new Board(width, height);
    }

    private void BuildObjects()
    {
        for (int z = 0; z < _currentBoard.Height; ++z)
        {
            for (int x = 0; x < _currentBoard.Width; ++x)
            {
                GameObject prefab = GetPrefabByType(CurrentBoard.GetTypeAt(x, z));
                GameObject go = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.identity);
                go.name = "cube_" + x + "_" + z;
                go.transform.SetParent(transform);

                _instantiatedCubes.Add(go);
            }
        }
    }


    private GameObject GetPrefabByType(Cell.CellType type)
    {
        GameObject result = null;
        switch (type)
        {
            case Cell.CellType.Empty:
                {
                    result = _emptyPrefab;
                }
                break;
            case Cell.CellType.Mine:
                {
                    result = _minePrefab;
                }
                break;
            case Cell.CellType.Start:
                {
                    result = _startPrefab;
                }
                break;
            case Cell.CellType.Exit:
                {
                    result = _exitPrefab;
                }
                break;
        }
        return result;
    }

    public void LoadNextLevel()
    {
    }

}
