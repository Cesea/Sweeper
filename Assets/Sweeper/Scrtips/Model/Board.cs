using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Board
{
    private Vector3Int _startCell;
    private Vector3Int _exitCell;

    public Vector3Int StartCellCoord
    {
        get { return _startCell; }
        set { _startCell = value; }
    }
    public Vector3Int ExitCellCoord
    {
        get { return _exitCell; }
        set { _exitCell = value; }
    }

    public Vector3 WorldSize;
    public float NodeRadius;

    private float _nodeDiameter;

    public Node[] Nodes;

    public int XCount { get; set; }
    public int YCount { get; set; }
    public int ZCount { get; set; }

    public int TotalCount { get { return XCount * YCount * ZCount; } }

    public Vector3 WorldStartPos;

    public GameObject BoardObject;


    private Material _material;

    public Board(Vector3 boardWorldPos, Vector3 worldSize, float nodeRadius, Material material)
    {
        _material = material;
        WorldStartPos = boardWorldPos;
        NodeRadius = nodeRadius;

        _nodeDiameter = nodeRadius * 2.0f;

        XCount = Mathf.RoundToInt(worldSize.x / _nodeDiameter);
        YCount = Mathf.RoundToInt(worldSize.y / _nodeDiameter);
        ZCount = Mathf.RoundToInt(worldSize.z / _nodeDiameter);

        BoardObject = new GameObject();
        BoardObject.transform.position = boardWorldPos;
        BuildNodes();
    }

    public void BuildNodes()
    {
        Nodes = new Node[TotalCount];

        for (int z = 0; z < ZCount; ++z)
        {
            for (int y = 0; y < YCount; ++y)
            {
                for (int x = 0; x < XCount; ++x)
                {
                    int index = Index3D(x, y, z);
                    Vector3 worldPos = WorldStartPos + new Vector3(
                        x * _nodeDiameter + NodeRadius,
                        y * _nodeDiameter + NodeRadius,
                        z * _nodeDiameter + NodeRadius );

                    Nodes[index] = new Node(x, y, z, worldPos, Node.NodeType.Empty, BoardObject, this);

                    if (y == 0)
                    {
                        Nodes[index].Type = Node.NodeType.Normal;
                    }
                }
            }
        }


        Nodes[Index3D(0, 1, 1)].Type = Node.NodeType.Normal;

        StartCellCoord = new Vector3Int(0, 0, 0);
        ExitCellCoord = new Vector3Int(5, 0, 5);

        Nodes[Index3D(StartCellCoord.x, StartCellCoord.y, StartCellCoord.z)].Type = Node.NodeType.Start;
        Nodes[Index3D(ExitCellCoord.x, ExitCellCoord.y, ExitCellCoord.z)].Type = Node.NodeType.Exit;
    }

    public void BuildMesh()
    {
        for (int z = 0; z < ZCount; ++z)
        {
            for (int y = 0; y < YCount; ++y)
            {
                for (int x = 0; x < XCount; ++x)
                {
                    Nodes[Index3D(x, y, z)].BuildMesh();
                }
            }
        }
        CombineQuads();

        MeshCollider collider = BoardObject.AddComponent<MeshCollider>();
        collider.sharedMesh = BoardObject.GetComponent<MeshFilter>().mesh;

        BoardObject.AddComponent<Rigidbody>().isKinematic = true;
    } 

    public bool CanMoveTo(int x, int y, int z)
    {
        bool result = true;
        if (!IsInBoundRef(ref x, ref y, ref z) || Nodes[Index3D(x, y, z)].IsSolid)
        {
            result = false;
            Debug.Log("movement out of range");
        }
        return result;
    }

    public void DestoryAllLevelObjects()
    {
        foreach (var n in Nodes)
        {
            foreach (var o in n.InstalledObjects)
            {
                GameObject.Destroy(o);
            }
        }
    }


    public void GetAdjacentCellsHorizontally(Vector3Int pos, bool includeCenter, ref Node[] cells)
    {
        GetAdjacentCellsHorizontally(pos.x, pos.y, pos.z, includeCenter, ref cells);
    }

    public void GetAdjacentCellsHorizontally(int x, int y, int z, bool includeCenter, ref Node[] cells)
    {
        int count = 0;
        for (int iz = z - 1; iz <= z + 1; ++iz)
        {
            for (int ix = x - 1; ix <= x + 1; ++ix)
            {
                int index = Index3D(ix, y, iz);

                if (!includeCenter && ix == x && iz == z)
                {
                    cells[count] = null;
                    continue;
                }

                if (IsInBoundRef(ref ix, ref y, ref iz))
                {
                    cells[count] = Nodes[index];
                }
                count++;
            }
        }
    }

    //public void GetAdjacentCells(int x, int z, ref Node[] cells)
    //{
    //    int count = 0;
    //    for (int iz = z - 1; iz <= z + 1; ++iz)
    //    {
    //        for (int ix = x - 1; ix <= x + 1; ++ix)
    //        {
    //            int index = ix + XCount * iz;
    //            if (ix == x && iz == z)
    //            {
    //                cells[count] = null;
    //                continue;
    //            }
    //            if (IsInBound(ix, iz))
    //            {
    //                cells[count] = _nodes[index];
    //            }
    //            count++;
    //        }
    //    }
    //}

    //private void SetAdjacentCells(int x, int y, int z, Node.NodeType type)
    //{
    //    for (int iz = z - 1; iz <= z + 1; ++iz)
    //    {
    //        for (int ix = x - 1; ix <= x + 1; ++ix)
    //        {
    //            int index = ix + XCount * iz;
    //            if (ix == x && iz == z)
    //            {
    //                continue;
    //            }
    //            if (IsInBound(ix, iz))
    //            {
    //                _nodes[index].Type = type;
    //            }
    //        }
    //    }
    //}

    #region  Utils

    public int Index3D(int x, int y, int z)
    {
        return x + (XCount * z) + (XCount * ZCount * y);
    }

    private void CombineQuads()
    {
        MeshFilter[] filters = BoardObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combines = new CombineInstance[filters.Length];

        for (int i = 0; i < filters.Length; ++i)
        {
            combines[i].mesh = filters[i].sharedMesh;
            combines[i].transform = filters[i].transform.localToWorldMatrix;
        }

        MeshFilter filter = BoardObject.AddComponent<MeshFilter>();
        filter.mesh = new Mesh();

        filter.mesh.CombineMeshes(combines);

        MeshRenderer renderer = BoardObject.AddComponent<MeshRenderer>();
        renderer.material = _material;

        foreach (Transform q in BoardObject.transform)
        {
            GameObject.Destroy(q.gameObject);
        }
    }

    public Node GetNodeAt(Vector3Int v)
    {
        return GetNodeAt(v.x, v.y, v.z);
    }

    public Node GetNodeAt(int x, int y, int z)
    {
        if (!IsInBoundRef(ref x, ref y, ref z))
        {
            return Nodes[Index3D(x, y, z)];
        }
        return Nodes[Index3D(x, y, z)];
    }

    public Node GetNodeAt(Vector3 worldPos)
    {
        int indexX = (int)((worldPos.x - WorldStartPos.x) / _nodeDiameter);
        int indexY = (int)((worldPos.y - WorldStartPos.y) / _nodeDiameter);
        int indexZ = (int)((worldPos.z - WorldStartPos.z) / _nodeDiameter);

        if (!IsInBoundRef(ref indexX, ref indexY, ref indexZ))
        {
            return Nodes[Index3D(indexX, indexY, indexZ)];
            //return null;
        }
        return Nodes[Index3D(indexX, indexY, indexZ)];

    }

    public Node.NodeType GetTypeAt(int x, int y, int z)
    {
        if (!IsInBoundRef(ref x, ref y, ref z))
        {
            return GetNodeAt(x, y, z).Type;
            //return Node.NodeType.Count;
        }
        return GetNodeAt(x, y, z).Type;
    }

    public Vector3Int GetRandomCellCoord()
    {
        return new Vector3Int((int)Random.Range(0, XCount), (int)Random.Range(0, YCount), (int)Random.Range(0, ZCount));
    }

    public Node GetOffsetedNode(Node node, int x, int y, int z)
    {
        return Nodes[Index3D(node.X + x, node.Y + y, node.Z + z)];
    }

    public bool IsInBoundRef(ref int x, ref int y, ref int z)
    {
        if (x < 0 || x > XCount - 1 ||
            y < 0 || y > YCount - 1 ||
            z < 0 || z > ZCount - 1)
        {
            Mathf.Clamp(x, 0, XCount - 1);
            Mathf.Clamp(y, 0, YCount - 1);
            Mathf.Clamp(z, 0, ZCount - 1);
            return false;
        }
        return true;
    }

    public bool IsInBound(int x, int y, int z)
    {
        if (x < 0 || x > XCount - 1 ||
               y < 0 || y > YCount - 1 ||
               z < 0 || z > ZCount - 1)
        {
            return false;
        }
        return true;
    }


    //일단은 횡으로만
    public List<NodeSideInfo> GetNeighbours(NodeSideInfo info)
    {
        int xDiff = 0;
        int yDiff = 0;
        int zDiff = 0;

        if (info._side == Side.Left || info._side == Side.Right)
        {
            yDiff = 1;
            zDiff = 1;
        }
        else if (info._side == Side.Front || info._side == Side.Back)
        {
            xDiff = 1;
            yDiff = 1;
        }
        else if (info._side == Side.Top || info._side == Side.Bottom)
        {
            xDiff = 1;
            zDiff = 1;
        }

        Node node = info._node;

        for (int z = node.Z - zDiff; z <= node.Z + zDiff; ++z)
        {
            for (int y = node.Y - yDiff; y <= node.Y + yDiff; ++y)
            {
                for (int x = node.X - xDiff; x <= node.X + xDiff; ++x)
                {
                    if (IsInBound(x, y, z))
                    {

                        Node currentNode = Nodes[Index3D(x, 0, z)];
                        if ((x == info._node.X && z == info._node.Z) || !currentNode.IsSolid)
                        {
                            continue;
                        }
                        result.Add(new NodeSideInfo(currentNode, Side.Top));

                    }
                }
            }
        }

        //List<NodeSideInfo> result = new List<NodeSideInfo>();
        ////forward
        //if (IsInBound(info._node.X, info._node.Y, info._node.Z + 1))
        //{
        //    Node currentNode = Nodes[Index3D(info._node.X, info._node.Y, info._node.Z + 1)];
        //    result.Add(new NodeSideInfo(currentNode, info._side));
        //}
        ////backward
        //if (IsInBound(info._node.X, info._node.Y, info._node.Z - 1))
        //{
        //    Node currentNode = Nodes[Index3D(info._node.X, info._node.Y, info._node.Z - 1)];
        //    result.Add(new NodeSideInfo(currentNode, info._side));
        //}
        ////left
        //if (IsInBound(info._node.X - 1, info._node.Y, info._node.Z))
        //{
        //    Node currentNode = Nodes[Index3D(info._node.X - 1, info._node.Y, info._node.Z)];
        //    result.Add(new NodeSideInfo(currentNode, info._side));
        //}
        ////right
        //if (IsInBound(info._node.X + 1, info._node.Y, info._node.Z))
        //{
        //    Node currentNode = Nodes[Index3D(info._node.X + 1, info._node.Y, info._node.Z)];
        //    result.Add(new NodeSideInfo(currentNode, info._side));
        //}
        //for (int z = info._node.Z - 1; z <= info._node.Z + 1; ++z)
        //{
        //    for (int x = info._node.X - 1; x <= info._node.X + 1; ++x)
        //    {
        //        if (IsInBound(x, 0, z))
        //        {
        //            Node currentNode = Nodes[Index3D(x, 0, z)];
        //            if ((x == info._node.X && z == info._node.Z) || !currentNode.IsSolid)
        //            {
        //                continue;
        //            }
        //            result.Add(new NodeSideInfo(currentNode, Side.Top));
        //        }
        //    }
        //}
        //return result;
    }

    #endregion

}
