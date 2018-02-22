using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class BoardManager : SingletonBase<BoardManager>
{
    [Header ("Cube Prefabs")]
    [SerializeField]
    GameObject _normalPrefab;

    [Header("Board Size")]

    public Vector3 WorldSize;
    public float NodeRadius = 0.5f;

    public int XCount { get; set; }
    public int YCount { get; set; }
    public int ZCount { get; set; }

    private Board _currentBoard;
    public Board CurrentBoard { get { return _currentBoard; } }

    private List<GameObject> _instantiatedCubes = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
    }

    public static Vector3 BoardPosToWorldPos(Vector3Int pos)
    {
        return Instance.transform.position + 
            new Vector3(pos.x * Instance.NodeRadius * 2.0f + Instance.NodeRadius,
                        pos.y * Instance.NodeRadius * 2.0f + Instance.NodeRadius,
                        pos.y * Instance.NodeRadius * 2.0f + Instance.NodeRadius);
    }

    public static Vector3Int WorldPosToBoardPos(Vector3 pos)
    {
        Vector3 relativePos = pos - Instance.transform.position;
        return new Vector3Int(
            (int)(relativePos.x / (Instance.NodeRadius * 2.0f)),
            (int)(relativePos.y / (Instance.NodeRadius * 2.0f)),
            (int)(relativePos.z / (Instance.NodeRadius * 2.0f)) );
    }

    public void BuildNewBoard()
    {
        XCount = Mathf.RoundToInt(WorldSize.x / (NodeRadius * 2.0f));
        YCount = Mathf.RoundToInt(WorldSize.y / (NodeRadius * 2.0f));
        ZCount = Mathf.RoundToInt(WorldSize.z / (NodeRadius * 2.0f));

        BuildBoard();
        BuildObjects();
    }

    //public GameObject GetObjectAt(int x, int y, int z)
    //{
    //    GameObject result = null;
    //    int index = Index3D(x, y, z);
    //    if (index < _instantiatedCubes.Count)
    //    {
    //        result = _instantiatedCubes[index];
    //    }
    //    return result;
    //}

    //public void SetObjectAt(int x, int y, int z, GameObject obj)
    //{
    //    int index = Index3D(x, y, z);
    //    if (index < _instantiatedCubes.Count)
    //    {
    //        _instantiatedCubes[index] = obj;
    //    }
    //}

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

        for (int z = 0; z < _currentBoard.ZCount; ++z)
        {
            for (int y = 0; y < _currentBoard.YCount; ++y)
            {
                for (int x = 0; x < _currentBoard.XCount; ++x)
                {
                    GameObject prefab = _normalPrefab;
                    GameObject go = Instantiate(prefab, new Vector3(x + NodeRadius, y + NodeRadius, z + NodeRadius), Quaternion.identity);
                    go.name = "cube_" + x + "_" + z;
                    go.transform.SetParent(transform);

                    _instantiatedCubes.Add(go);
                }
            }
        }
    }

    #region Utils

    private int Index3D(int x, int y, int z)
    {
        return x + (XCount * z) + (XCount * ZCount * y);
    }

    private GameObject GetPrefabByType(Node.NodeType type)
    {
        GameObject result = null;
        switch (type)
        {
            case Node.NodeType.Empty:
                {
                    //result = _normalPrefab;
                }
                break;
            case Node.NodeType.Normal:
                {
                    result = _normalPrefab;
                }
                break;
        }
        return result;
    }

    #endregion

}
