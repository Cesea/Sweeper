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

    public void BuildNewBoard(int width, int height)
    {
        BuildBoard(width, height);
        BuildObjects();
    }

    public GameObject GetObjectAt(int x, int z)
    {
        GameObject result = null;
        int index = x + _width * z;
        if (index < _instantiatedCubes.Count)
        {
            result = _instantiatedCubes[index];
        }
        return result;
    }

    public void SetObjectAt(int x, int z, GameObject obj)
    {
        int index = x + _width * z;
        if (index < _instantiatedCubes.Count)
        {
            _instantiatedCubes[index] = obj;
        }
    }


    private void BuildBoard(int width, int height)
    {
        _currentBoard = new Board(width, height);
    }

    private void BuildObjects()
    {
        if (_instantiatedCubes.Count > 0)
        {
            for (int i = _instantiatedCubes.Count - 1; i >= 0; --i)
            {
                Destroy(_instantiatedCubes[i]);
            }
            _instantiatedCubes.Clear();
        }

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

}
