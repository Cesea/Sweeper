using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class BoardManager : SingletonBase<BoardManager>
{
    [Header("Floor Material")]
    [SerializeField]
    private Material _floorMaterial;
    [Header("Floor Objects")]
    public GameObject _stoneShortPrefab;
    public GameObject _stoneTallPrefab;

    [Header("Board Size")]

    public Vector3 WorldSize;
    public float NodeRadius = 0.5f;

    public int XCount { get; set; }
    public int YCount { get; set; }
    public int ZCount { get; set; }

    private Board _currentBoard;
    public Board CurrentBoard { get { return _currentBoard; } }

    private List<GameObject> _boardVisualObjects = new List<GameObject>();

    static float EPSILON = 0.001f;

    protected override void Awake()
    {
        _instance = this;
    }

    public static Vector3 BoardPosToWorldPos(Vector3Int pos)
    {
        return Instance.transform.position + 
            new Vector3(pos.x * Instance.NodeRadius * 2.0f + Instance.NodeRadius,
                        pos.y * Instance.NodeRadius * 2.0f + Instance.NodeRadius,
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
        BuildBoardCollisionMesh();
        BuildBoardVisualMesh();
    }

    private void BuildBoard()
    {
        if (_currentBoard != null)
        {
            _currentBoard.DestoryAllLevelObjects();
            GameObject.Destroy(_currentBoard.CollisionObject);
        }
        _currentBoard = new Board(transform.position, WorldSize, NodeRadius);
        _currentBoard.BuildNodes();
    }

    private void BuildBoardCollisionMesh()
    {
        _currentBoard.BuildMesh();
    }

    private void BuildBoardVisualMesh()
    {
        Node[] nodeDatas = CurrentBoard.Nodes;
        Node[] visualNodes = new Node[XCount * YCount * ZCount];
        Board visualBoard = new Board(transform.position, WorldSize, NodeRadius);

        visualBoard.Nodes = visualNodes;

        GameObject floorObject = new GameObject("Floor");
        //Get Data from original nodes
        for (int z = 0; z < ZCount; ++z)
        {
            for (int y = 0; y < YCount; ++y)
            {
                for (int x = 0; x < XCount; ++x)
                {
                    Node currentData = nodeDatas[Index3D(x, y, z)];
                    Node.NodeType type = (y == 0) ? Node.NodeType.Normal : Node.NodeType.Empty;
                    visualNodes[Index3D(x, y, z)] = new Node(x, y, z, currentData.WorldPosition, type, floorObject, visualBoard);
                }
            }
        }

        for (int z = 0; z < ZCount; ++z)
        {
            for (int x = 0; x < XCount; ++x)
            {
                visualNodes[Index3D(x, 0, z)].BuildMesh(floorObject);
            }
        }
        Utils.MeshBuilder.CombineQuad(floorObject);
        MeshRenderer renderer = floorObject.AddComponent<MeshRenderer>();
        renderer.material = _floorMaterial;

        _boardVisualObjects.Add(floorObject);

        //Build upper floor
        List<Node> upperNodeList = new List<Node>();
        for (int z = 0; z < ZCount; ++z)
        {
            for (int x = 0; x < XCount; ++x)
            {
                for (int y = 1; y < YCount; ++y)
                {
                    if (nodeDatas[Index3D(x, y, z)].IsSolid)
                    {
                        upperNodeList.Add(nodeDatas[Index3D(x, y, z)]);
                    }
                }
                if (upperNodeList.Count == 1)
                {
                    _boardVisualObjects.Add(
                        Level.LevelCreator.Instance.CreateObjectAtNode(
                        CurrentBoard.GetNodeInfoAt(x, 0, z, Side.Top), _stoneShortPrefab));
                }
                else if (upperNodeList.Count == 2)
                {
                    _boardVisualObjects.Add(
                        Level.LevelCreator.Instance.CreateObjectAtNode(
                        CurrentBoard.GetNodeInfoAt(x, 0, z, Side.Top), _stoneTallPrefab));
                }
                upperNodeList.Clear();
            }
        }


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


    public static Vector3Int SideToOffset(Side side)
    {
        Vector3Int result = new Vector3Int();
         switch (side)
        {
            case Side.Left:
                {
                    result.x = -1;
                } break;
            case Side.Right :
                {
                    result.x = 1;
                } break;
            case Side.Top :
                {
                    result.y = 1;
                } break;
            case Side.Bottom :
                {
                    result.y = -1;
                } break;
            case Side.Front :
                {
                    result.z = +1;
                } break;
            case Side.Back :
                {
                    result.z = -1;
                } break;
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
                    result = Vector3.left * Instance.NodeRadius;
                } break;
            case Side.Right :
                {
                    result = Vector3.right * Instance.NodeRadius;
                } break;
            case Side.Top :
                {
                    result = Vector3.up * Instance.NodeRadius;
                } break;
            case Side.Bottom :
                {
                    result = Vector3.down * Instance.NodeRadius;
                } break;
            case Side.Front :
                {
                    result = Vector3.forward * Instance.NodeRadius;
                } break;
            case Side.Back :
                {
                    result = Vector3.back * Instance.NodeRadius;
                } break;
        }
        return result;
    }

    public static Quaternion SideToRotation(Side side)
    {
        Quaternion result = Quaternion.identity;
        switch (side)
        {
            case Side.Left:
                {
                    result = Quaternion.Euler(0, 0, 90);
                } break;
            case Side.Right :
                {
                    result = Quaternion.Euler(0, 0, -90);
                } break;
            case Side.Top :
                {
                    result = Quaternion.identity;
                } break;
            case Side.Bottom :
                {
                    result = Quaternion.Euler(180, 0, 0);
                } break;
            case Side.Front :
                {
                    result = Quaternion.Euler(90, 0, 0);
                } break;
            case Side.Back :
                {
                    result = Quaternion.Euler(-90, 0, 0);
                } break;
        }
        return result;
    }

    public static bool GetNodeSideInfoAtMouse(ref NodeSideInfo info)
    {
        bool result = false;
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Instance.CurrentBoard.CollisionObject.GetComponent<MeshCollider>().Raycast(camRay, out hitInfo, 1000.0f))
        {
            Vector3 roundedHitPos = hitInfo.point;
            GameStateManager.Instance.MouseNodeSidePosition = hitInfo.point;
            if (Mathf.Abs(hitInfo.point.x - Mathf.RoundToInt(hitInfo.point.x)) < EPSILON)
            {
                roundedHitPos.x = Mathf.RoundToInt(hitInfo.point.x);
            }
            if (Mathf.Abs(hitInfo.point.y - Mathf.RoundToInt(hitInfo.point.y)) < EPSILON)
            {
                roundedHitPos.y = Mathf.RoundToInt(hitInfo.point.y);
            }
            if (Mathf.Abs(hitInfo.point.z - Mathf.RoundToInt(hitInfo.point.z)) < EPSILON)
            {
                roundedHitPos.z = Mathf.RoundToInt(hitInfo.point.z);
            }

            Vector3Int tmp = BoardManager.WorldPosToBoardPos(roundedHitPos);

            if (hitInfo.normal.y > 0)
            {
                tmp.y -= 1;
            }
            else if (hitInfo.normal.x > 0)
            {
                tmp.x -= 1;
            }
            else if (hitInfo.normal.z > 0)
            {
                tmp.z -= 1;
            }
            NodeSideInfo tmpInfo = Instance.CurrentBoard.GetNodeInfoAt(tmp.x, tmp.y, tmp.z, NormalToSide(hitInfo.normal));
            if (!Object.ReferenceEquals(tmpInfo, null))
            {
                info = tmpInfo;
            }
            result = true;
        }
        return result;
    }

    #endregion

}
