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

[System.Serializable]
public class Node
{
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

    public bool IsSolid { get; set; }

    public Vector3 WorldPosition;

    private GameObject _parent;
    private Board _owner;

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

    public Vector3Int BoardPosition
    {
        get { return new Vector3Int(_x, _y, _z); }
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
                        IsSolid = false;
                    } break;
                case NodeType.Normal:
                    {
                        IsSolid = true;
                    } break;
                case NodeType.Start:
                    {
                        IsSolid = true;
                    } break;
                case NodeType.Exit:
                    {
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
        Mesh mesh = Utils.MeshBuilder.BuildQuad(side, BoardManager.Instance.NodeRadius, Vector3.zero);

        Vector2[] uvs = new Vector2[4];

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

        uvs = new Vector2[] { uv00, uv10, uv11, uv01 };

        mesh.uv = uvs;

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

    public Node GetNodeAtSide(Side side)
    {
        Node result = null;
        Vector3Int offset = BoardManager.SideToOffset(side);
        Vector3Int testPos = new Vector3Int(X + offset.x, Y + offset.y, Z + offset.z);
        if (BoardManager.Instance.CurrentBoard.IsInBound(testPos.x, testPos.y, testPos.z))
        {
            result = BoardManager.Instance.CurrentBoard.GetNodeAt(testPos);
        }
        return result;
    }

    public override bool Equals(object obj)
    {
        Node node = obj as Node;
        if (Object.ReferenceEquals(node, null))
        {
            return false;
        }
        else
        {
            return (X == node.X && Y == node.Y && Z == node.Z);
        }
    }

    public static bool operator ==(Node a, Node b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Node a, Node b)
    {
        return !a.Equals(b);
    }

}

public class NodeSideInfo
{
    public int HCost { get; set; }
    public int GCost { get; set; }

    public int TotalCost { get { return HCost + GCost; } }

    public Node _node { get; set; }
    public Side _side { get; set; }

    public bool IsHazard { get; set; }
    public bool IsPassable { get; set; }

    public GameObject _sittingObject = null;
    public GameObject _installedObject = null;

    public NodeSideInfo Parent;

    public override bool Equals(object obj)
    {
        NodeSideInfo info = obj as NodeSideInfo;
        if (Object.ReferenceEquals(obj, null))
        {
            return false;
        }
        else
        {
            return ((_node == info._node) && (_side == info._side));
        }
    }

    public NodeSideInfo()
    {
        _node = null;
        _side = Side.Count;

    }

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

    public static bool operator ==(NodeSideInfo a, NodeSideInfo b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(NodeSideInfo a, NodeSideInfo b)
    {
        return !(a.Equals(b));
    }

    public Node GetNodeAtSide()
    {
        return _node.GetNodeAtSide(_side);
    }

    public static Vector3 GetClosestEdge(NodeSideInfo a, NodeSideInfo b)
    {
        Vector3 result = Vector3.zero;
        Vector3 aPos = a.GetWorldPosition();
        Vector3 bPos = b.GetWorldPosition();

        if (a._side == b._side)
        {
            Vector3 diffHalf = (bPos - aPos) * 0.5f;
            result = aPos + diffHalf;
        }
        else
        {
            Vector3 diff = (bPos - aPos);
            result = a._node.WorldPosition + diff;
        }

        return result;

    }

}
