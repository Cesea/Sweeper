using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Side
{
    Left,
    Right,
    Top,
    Bottom,
    Front,
    Back,
    Count
}

public class Node
{
    //LevelObject
    public GameObject[] InstalledObjects;
    //BoardObject
    public GameObject[] SittingObjects;

    public GameObject GetInstalledObjectAt(Side side)
    {
        return InstalledObjects[(int)side];
    }
    public GameObject GetSittingObjectAt(Side side)
    {
        return SittingObjects[(int)side];
    }
    public void SetInstalledObjectAt(Side side, GameObject o)
    {
        InstalledObjects[(int)side] = o;
    }
    public void SetSittingObjectAt(Side side, GameObject o)
    {
        SittingObjects[(int)side] = o;
    }

    public enum NodeType
    {
        Normal,
        Start,
        Exit,
        Empty,
        Count
    }

    private NodeType _type;
    private int _x;
    private int _y;
    private int _z;

    public bool IsHazard { get; set; }
    public bool IsPassable { get; set; }

    public bool IsSolid { get; set; }

    public Vector3 WorldPosition;

    private GameObject _parent;
    private Board _owner;

    public int GCost { get; set; }
    public int HCost { get; set; }
    public int TotalCost { get { return GCost + HCost; } }

    public Node Parent { get; set; }

    public int X
    {
        get { return _x; }
        set { _x = value; }
    }
    public int Y
    {
        get { return _y; }
        set { _y = value; }
    }
    public int Z
    {
        get { return _z; }
        set { _z = value; }
    }

    public NodeType Type
    {
        get { return _type; }
        set
        {
            _type = value;
            switch (_type)
            {
                case NodeType.Empty:
                    {
                        IsHazard = true;
                        IsPassable = true;
                        IsSolid = false;
                    } break;
                case NodeType.Normal:
                    {
                        IsHazard = false;
                        IsPassable = true;
                        IsSolid = true;
                    } break;
                case NodeType.Start:
                    {
                        IsHazard = false;
                        IsPassable = true;
                        IsSolid = true;
                    } break;
                case NodeType.Exit:
                    {
                        IsHazard = false;
                        IsPassable = true;
                        IsSolid = true;
                    } break;
            }
        }
    }

    public static Vector2[,] BlockUVs =
    {
        { new Vector2(0.0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.0f, 1.0f), new Vector2(0.5f, 1.0f) }, 
        { new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.0f), new Vector2(0.0f, 0.5f), new Vector2(0.5f, 0.5f) }, 
        { new Vector2(0.5f, 0.0f), new Vector2(1.0f, 0.0f), new Vector2(0.5f, 0.5f), new Vector2(1.0f, 0.5f) }, 
        { new Vector2(0.5f, 0.5f), new Vector2(1.0f, 0.5f), new Vector2(0.5f, 1.0f), new Vector2(1.0f, 1.0f) }, 
    };

    public Node(int x, int y, int z, Vector3 worldPos, NodeType type, GameObject parent, Board owner)
    {
        _x = x;
        _y = y;
        _z = z;
        _type = type;
        WorldPosition = worldPos;
        _parent = parent;
        _owner = owner;

        InstalledObjects = new GameObject[(int)Side.Count];
        SittingObjects = new GameObject[(int)Side.Count];
    }

    public void BuildMesh()
    {
        if (!IsSolid)
        {
            return;
        }
        if (!HasSolidNeighbour((int)WorldPosition.x, (int)WorldPosition.y - 1, (int)WorldPosition.z))
            BuildQuad(Side.Bottom);
        if (!HasSolidNeighbour((int)WorldPosition.x, (int)WorldPosition.y + 1, (int)WorldPosition.z))
            BuildQuad(Side.Top);
        if (!HasSolidNeighbour((int)WorldPosition.x- 1, (int)WorldPosition.y, (int)WorldPosition.z))
            BuildQuad(Side.Left);
        if (!HasSolidNeighbour((int)WorldPosition.x + 1, (int)WorldPosition.y, (int)WorldPosition.z))
            BuildQuad(Side.Right);
        if (!HasSolidNeighbour((int)WorldPosition.x, (int)WorldPosition.y, (int)WorldPosition.z + 1))
            BuildQuad(Side.Front);
        if (!HasSolidNeighbour((int)WorldPosition.x, (int)WorldPosition.y, (int)WorldPosition.z - 1))
            BuildQuad(Side.Back);
    }

    private void BuildQuad(Side side)
    {
        Mesh mesh = new Mesh();
        mesh.name = "Scripted Mesh";

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];

        int[] triangles = new int[6];

        Vector3 p1 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 p2 = new Vector3(-0.5f, 0.5f, -0.5f);
        Vector3 p3 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 p4 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 p5 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 p6 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 p7 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 p8 = new Vector3(0.5f, -0.5f, 0.5f);

        Vector2 uv00 = new Vector2(0.0f, 0.0f);
        Vector2 uv10 = new Vector2(1.0f, 0.0f);
        Vector2 uv01 = new Vector2(0.0f, 1.0f);
        Vector2 uv11 = new Vector2(1.0f, 1.0f);

        if (_type == NodeType.Normal)
        {
            uv00 = BlockUVs[(int)(_type), 0];
            uv10 = BlockUVs[(int)(_type), 1];
            uv01 = BlockUVs[(int)(_type), 2];
            uv11 = BlockUVs[(int)(_type), 3];
        }
        else if (_type == NodeType.Start)
        {
            uv00 = BlockUVs[(int)(_type), 0];
            uv10 = BlockUVs[(int)(_type), 1];
            uv01 = BlockUVs[(int)(_type), 2];
            uv11 = BlockUVs[(int)(_type), 3];
        }
        else if(_type == NodeType.Exit)
        {
            uv00 = BlockUVs[(int)(_type), 0];
            uv10 = BlockUVs[(int)(_type), 1];
            uv01 = BlockUVs[(int)(_type), 2];
            uv11 = BlockUVs[(int)(_type), 3];
        }

        switch (side)
        {
            case Side.Bottom:
                {
                    vertices = new Vector3[] { p1, p4, p8, p5 };
                    normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                    uvs = new Vector2[] { uv00, uv10, uv11, uv01 };
                    triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                }
                break;
            case Side.Top:
                {
                    vertices = new Vector3[] { p2, p6, p7, p3 };
                    normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                    uvs = new Vector2[] { uv00, uv01, uv11, uv10 };
                    triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                }
                break;
            case Side.Left:
                {
                    vertices = new Vector3[] { p1, p5, p6, p2 };
                    normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                    uvs = new Vector2[] { uv00, uv10, uv11, uv01 };
                    triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                }
                break;
            case Side.Right:
                {
                    vertices = new Vector3[] { p4, p3, p7, p8 };
                    normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                    uvs = new Vector2[] { uv00, uv01, uv11, uv10 };
                    triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                }
                break;
            case Side.Front:
                {
                    vertices = new Vector3[] { p5, p8, p7, p6 };
                    normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                    uvs = new Vector2[] { uv00, uv10, uv11, uv01 };
                    triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                }
                break;
            case Side.Back:
                {
                    vertices = new Vector3[] { p1, p2, p3, p4 };
                    normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                    uvs = new Vector2[] { uv00, uv01, uv11, uv10 };
                    triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                }
                break;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GameObject go = new GameObject(side.ToString());
        go.transform.position = WorldPosition;
        go.transform.SetParent(_parent.transform);

        MeshFilter filter = go.AddComponent<MeshFilter>();
        filter.mesh = mesh;
    }

    public bool HasSolidNeighbour(int x, int y, int z)
    {
        Node[] nodeData;
        if (x < 0 || x > _owner.XCount - 1 ||
            y < 0 || y > _owner.YCount - 1 ||
            z < 0 || z > _owner.ZCount - 1)
        {
            return false;
        }
        else
        {
            nodeData = _owner.Nodes;
        }

        try
        {
            return nodeData[_owner.Index3D(x, y, z)].IsSolid;
        }
        catch (System.IndexOutOfRangeException ex) { }
        return false;
    }

    public Vector3 GetWorldPositionBySide(Side side)
    {
        return WorldPosition + BoardManager.SideToVector3Offset(side);
    }
}

public struct NodeSideInfo
{
    public Node _node { get; set; }
    public Side _side { get; set; }

    public NodeSideInfo(Node node, Side side)
    {
        _node = node;
        _side = side;
    }

    public Vector3 GetWorldPosition()
    {
        if (_node == null)
        {
            Debug.Log("null");
        }
        return _node.GetWorldPositionBySide(_side);
    }

    public static bool operator !=(NodeSideInfo a, NodeSideInfo b)
    {
        return !(a == b);
    }

    public static bool operator ==(NodeSideInfo a, NodeSideInfo b)
    {
        return a._node == b._node && a._side == b._side;
    }
}
