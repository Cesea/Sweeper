using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class BoardManager : SingletonBase<BoardManager>
{
    [Header("Material")]
    public Material _material;

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
                        pos.y * Instance.NodeRadius * 2.0f /*+ Instance.NodeRadius*/,
                        pos.z * Instance.NodeRadius * 2.0f + Instance.NodeRadius);
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
        BuildBoardMesh();
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

        _currentBoard = new Board(transform.position, WorldSize, NodeRadius, _material);
    }

    //private void BuildObjects()
    //{
    //    if (_instantiatedCubes.Count > 0)
    //    {
    //        for (int i = _instantiatedCubes.Count - 1; i >= 0; --i)
    //        {
    //            Destroy(_instantiatedCubes[i]);
    //        }
    //        _instantiatedCubes.Clear();
    //    }

    //    for (int z = 0; z < _currentBoard.ZCount; ++z)
    //    {
    //        for (int y = 0; y < _currentBoard.YCount; ++y)
    //        {
    //            for (int x = 0; x < _currentBoard.XCount; ++x)
    //            {
    //                GameObject prefab = GetPrefabByType(CurrentBoard.GetNodeAt(x, y, z).Type);
    //                if (prefab != null)
    //                {
    //                    GameObject go = Instantiate(prefab, new Vector3(x + NodeRadius, y + NodeRadius, z + NodeRadius), Quaternion.identity);
    //                    go.name = "cube_" + x + "_" + z;
    //                    go.transform.SetParent(transform);
    //                    _instantiatedCubes.Add(go);
    //                }
    //            }
    //        }
    //    }
    //}

    private void BuildBoardMesh()
    {
        _currentBoard.BuildMesh();
    }

    #region Utils

    private int Index3D(int x, int y, int z)
    {
        return x + (XCount * z) + (XCount * ZCount * y);
    }

    public static Side NormalToSide(Vector3 v)
    {
        Side result = Side.Top;

        if (v.y > 0)
        {
            result = Side.Top;
        }
        else if (v.y < 0)
        {
            result = Side.Bottom;
        }
        else if (v.x > 0)
        {
            result = Side.Right;
        }
        else if (v.x < 0)
        {
            result = Side.Left;
        }
        else if (v.z > 0)
        {
            result = Side.Front;
        }
        else if (v.z < 0)
        {
            result = Side.Back;
        }
        return result;
    }

    public static Vector3 SideToVector3Offset(Side side)
    {
        Vector3 result = Vector3.zero;
        switch (side)
        {
            case Side.Left:
                {
                    result = Vector3.left;
                } break;
            case Side.Right :
                {
                    result = Vector3.right;
                } break;
            case Side.Top :
                {
                    result = Vector3.up;
                } break;
            case Side.Bottom :
                {
                    result = Vector3.down;
                } break;
            case Side.Front :
                {
                    result = Vector3.forward;
                } break;
            case Side.Back :
                {
                    result = Vector3.back;
                } break;
        }
        return result;
    }

    //일단 오브젝트가 있는 땅이면 못간다....
    public static Node GetNodeAtMouse(ref Side side, ref Vector3 gridPos)
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        {
            RaycastHit hitInfo;
            if (Instance.CurrentBoard.BoardObject.GetComponent<MeshCollider>().Raycast(camRay, out hitInfo, 1000.0f))
            {
                Vector3Int boardCoord = BoardManager.WorldPosToBoardPos(hitInfo.point);
                gridPos = BoardManager.BoardPosToWorldPos(boardCoord);
                //Upper Side
                if (hitInfo.normal.y > 0)
                {
                    boardCoord.y -= 1;
                }
                else if (hitInfo.normal.x > 0)
                {
                    boardCoord.x -= 1;
                }
                else if (hitInfo.normal.z > 0)
                {
                    boardCoord.z -= 1;
                }

                side = NormalToSide(hitInfo.normal);
                Node node = BoardManager.Instance.CurrentBoard.GetNodeAt(boardCoord);
                if (node != null)
                {
                    return node;
                }
            }
        }
        return null;
    }

    #endregion

}
