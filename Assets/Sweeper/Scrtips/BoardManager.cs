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

    public Vector2 WorldSize;
    public float NodeRadius = 0.5f;

    private int _width;
    private int _height;

    private Board _currentBoard;
    public Board CurrentBoard { get { return _currentBoard; } }

    private List<GameObject> _instantiatedCubes = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
    }

    public static Vector3 BoardPosToWorldPos(Vector2Int pos)
    {
        return Instance.transform.position + new Vector3(pos.x * Instance.NodeRadius * 2.0f + Instance.NodeRadius, 
            0, 
            pos.y * Instance.NodeRadius * 2.0f + Instance.NodeRadius);
    }

    public static Vector2Int WorldPosToBoardPos(Vector3 pos)
    {
        Vector3 relativePos = pos - Instance.transform.position;
        return new Vector2Int((int)(relativePos.x / (Instance.NodeRadius * 2.0f)), (int)(relativePos.z / (Instance.NodeRadius * 2.0f)));
    }

    public void BuildNewBoard(Vector2 worldSize, float nodeRadius)
    {
        WorldSize = worldSize;
        NodeRadius = nodeRadius;
        _width = Mathf.RoundToInt(WorldSize.x / (NodeRadius * 2.0f));
        _height = Mathf.RoundToInt(WorldSize.y / (NodeRadius * 2.0f));

        BuildBoard();
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

    private void BuildBoard()
    {
        if (_currentBoard != null)
        {
            _currentBoard.DestoryAllLevelObjects();
        }
        _currentBoard = new Board(transform.position, WorldSize, NodeRadius);
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

        for (int z = 0; z < _currentBoard.YCount; ++z)
        {
            for (int x = 0; x < _currentBoard.XCount; ++x)
            {
                GameObject prefab = GetPrefabByType(CurrentBoard.GetTypeAt(x, z));
                GameObject go = Instantiate(prefab, new Vector3(x + NodeRadius, 0, z + NodeRadius), Quaternion.identity);
                go.name = "cube_" + x + "_" + z;
                go.transform.SetParent(transform);

                _instantiatedCubes.Add(go);
            }
        }
    }


    private GameObject GetPrefabByType(Node.NodeType type)
    {
        GameObject result = null;
        switch (type)
        {
            case Node.NodeType.Empty:
                {
                    result = _emptyPrefab;
                }
                break;
            case Node.NodeType.Mine:
                {
                    result = _minePrefab;
                }
                break;
            case Node.NodeType.Start:
                {
                    result = _startPrefab;
                }
                break;
            case Node.NodeType.Exit:
                {
                    result = _exitPrefab;
                }
                break;
        }
        return result;
    }

}
