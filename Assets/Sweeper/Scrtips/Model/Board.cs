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

    private NodeSideInfo[,] _nodeSideInfos;

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
        //Node Building
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
        Nodes[Index3D(0, 1, 2)].Type = Node.NodeType.Normal;
        Nodes[Index3D(0, 2, 2)].Type = Node.NodeType.Normal;

        StartCellCoord = new Vector3Int(0, 0, 0);
        ExitCellCoord = new Vector3Int(5, 0, 5);

        Nodes[Index3D(StartCellCoord.x, StartCellCoord.y, StartCellCoord.z)].Type = Node.NodeType.Start;
        Nodes[Index3D(ExitCellCoord.x, ExitCellCoord.y, ExitCellCoord.z)].Type = Node.NodeType.Exit;

        //SideInfo Building ////////////////////
        _nodeSideInfos = new NodeSideInfo[TotalCount, 6];

        for (int z = 0; z < ZCount; ++z)
        {
            for (int y = 0; y < YCount; ++y)
            {
                for (int x = 0; x < XCount; ++x)
                {
                    if (Nodes[Index3D(x, y, z)].IsSolid)
                    {
                        for (int i = 0; i < (int)Side.Count; ++i)
                        {
                            _nodeSideInfos[Index3D(x, y, z), i] = new NodeSideInfo(Nodes[Index3D(x, y, z)], (Side)i);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

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

    public NodeSideInfo GetClosesetNodeSideInfo(NodeSideInfo info, Node target)
    {
        if (info._node == target)
        {
            return null;
        }

        int deltaX = target.X - info._node.X;
        int deltaY = target.Y - info._node.Y;
        int deltaZ = target.Z - info._node.Z;

        if (deltaX >= 2 || deltaY >= 2 || deltaZ >= 2)
        {
            return null;
        }

        int closest = 0;
        float closestDistance = 999.0f;
        for (int i = 0; i < (int)Side.Count; ++i)
        {
            Vector3 targetSidePosition = target.GetWorldPositionBySide((Side)i);
            float distance = Vector3.Distance(targetSidePosition, info.GetWorldPosition());
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = i;
            }
        }

        return _nodeSideInfos[Index3D(target.X, target.Y, target.Z), closest];
    }

    public List<NodeSideInfo> GetReachables(NodeSideInfo info)
    {
        List<NodeSideInfo> result = new List<NodeSideInfo>();
        Node node = info._node;

        int minusX = 0;
        int plusX = 0;
        int minusY = 0;
        int plusY = 0;
        int minusZ = 0;
        int plusZ = 0;

        int localUp = 0;

        switch (info._side)
        {
            case Side.Back:
            case Side.Front:
                {
                    minusX = -1;
                    plusX = 1;
                    minusY = -1;
                    plusY = 1;

                    localUp = (info._side == Side.Back) ? -1 : 1;
                }
                break;

            case Side.Left:
            case Side.Right:
                {
                    minusY = -1;
                    plusY = 1;
                    minusZ = -1;
                    plusZ = 1;
                    localUp = (info._side == Side.Left) ? -1 : 1;
                }
                break;

            case Side.Top:
            case Side.Bottom:
                {
                    minusX = -1;
                    plusX = 1;
                    minusZ = -1;
                    plusZ = 1;
                    localUp = (info._side == Side.Bottom) ? -1 : 1;
                }
                break;
        }

        int minX = node.X + minusX;
        int maxX = node.X + plusX;
        int minY = node.Y + minusY;
        int maxY = node.Y + plusY;
        int minZ = node.Z + minusZ;
        int maxZ = node.Z + plusZ;

        Bounds upBound = new Bounds(
                        info.GetWorldPosition() + BoardManager.SideToVector3Offset(info._side),
                        new Vector3(BoardManager.Instance.NodeRadius * 2.1f,
                                    BoardManager.Instance.NodeRadius * 2.1f,
                                    BoardManager.Instance.NodeRadius * 2.1f));
        switch (info._side)
        {
            case Side.Back:
            case Side.Front:
                {
                    for (int y = minY; y <= maxY; ++y)
                    {
                        for (int x = minX; x <= maxX; ++x)
                        {
                            if (x == minX && y == minY ||
                               x == maxX && y == minY ||
                               x == minX && y == maxY ||
                               x == maxX && y == maxY)
                            {
                                continue;
                            }
                            bool topExist = false;

                            int diffX = x - node.X;
                            int diffY = y - node.Y;
                            int diffZ = 0;

                            if (IsInBound(x, y, node.Z + localUp))
                            {
                                Node currentNode = Nodes[Index3D(x, y,  node.Z + localUp)];
                                NodeSideInfo closest = GetClosesetNodeSideInfo(info, currentNode);
                                if (currentNode.IsSolid)
                                {
                                    topExist = true;
                                    if (upBound.Contains(currentNode.GetWorldPositionBySide(closest._side)))
                                    {
                                        result.Add(closest);
                                    }
                                }
                            }

                            if (topExist)
                            {
                                continue;
                            }

                            if (IsInBound(x, y, node.Z))
                            {
                                Node currentNode = Nodes[Index3D(x, y, node.Z)];
                                if (currentNode.IsSolid)
                                {
                                    result.Add(GetNodeInfoAt(currentNode.X, currentNode.Y, currentNode.Z, info._side));
                                }
                                else
                                {
                                    result.Add(GetNodeInfoAt(node.X, node.Y, node.Z,
                                        BoardManager.NormalToSide(new Vector3(diffX, diffY, diffZ))));
                                }
                            }
                        }
                    }
                }
                break;

            case Side.Left:
            case Side.Right:
                {
                    for (int y = minY; y <= maxY; ++y)
                    {
                        for (int z = minZ; z <= maxZ; ++z)
                        {
                            if (y == minY && z == minZ ||
                                y == maxY && z == minZ ||
                                y == minY && z == maxZ ||
                                y == maxY && z == maxZ)
                            {
                                continue;
                            }
                            bool topExist = false;

                            int diffX = 0;
                            int diffY = y - node.Y;
                            int diffZ = z - node.Z;

                            if (IsInBound(node.X + localUp, y, z))
                            {
                                Node currentNode = Nodes[Index3D(node.X + localUp, y, z)];
                                NodeSideInfo closest = GetClosesetNodeSideInfo(info, currentNode);
                                if (currentNode.IsSolid)
                                {
                                    topExist = true;
                                    if (upBound.Contains(currentNode.GetWorldPositionBySide(closest._side)))
                                    {
                                        result.Add(closest);
                                    }
                                }
                            }

                            if (topExist)
                            {
                                continue;
                            }

                            if (IsInBound(node.X, y, z))
                            {
                                Node currentNode = Nodes[Index3D(node.X, y, z)];
                                if (currentNode.IsSolid)
                                {
                                    result.Add(GetNodeInfoAt(currentNode.X, currentNode.Y, currentNode.Z, info._side));
                                }
                                else
                                {
                                    result.Add(GetNodeInfoAt(node.X, node.Y, node.Z,
                                        BoardManager.NormalToSide(new Vector3(diffX, diffY, diffZ))));
                                }
                            }
                        }
                    }
                }
                break;

            case Side.Top:
            case Side.Bottom:
                {
                    for (int z = minZ; z <= maxZ; ++z)
                    {
                        for (int x = minX; x <= maxX; ++x)
                        {
                            if (x == minX && z == minZ ||
                                x == maxX && z == minZ ||
                                x == minX && z == maxZ ||
                                x == maxX && z == maxZ)
                            {
                                continue;
                            }
                            bool topExist = false;

                            int diffX = x - node.X;
                            int diffY = 0;    
                            int diffZ = z - node.Z;

                            if (IsInBound(x, node.Y + 1, z))
                            {
                                Node currentNode = Nodes[Index3D(x, node.Y + 1, z)];
                                NodeSideInfo closest = GetClosesetNodeSideInfo(info, currentNode);
                                if (currentNode.IsSolid)
                                {
                                    topExist = true;
                                    if (upBound.Contains(currentNode.GetWorldPositionBySide(closest._side)))
                                    {
                                        result.Add(closest);
                                    }
                                }
                            }

                            if (topExist)
                            {
                                continue;
                            }

                            if (IsInBound(x, node.Y, z))
                            {
                                Node currentNode = Nodes[Index3D(x, node.Y, z)];
                                if (currentNode.IsSolid)
                                {
                                    result.Add(GetNodeInfoAt(currentNode.X, currentNode.Y, currentNode.Z, info._side));
                                }
                                else
                                {
                                    result.Add(GetNodeInfoAt(node.X, node.Y, node.Z,
                                        BoardManager.NormalToSide(new Vector3(diffX, diffY, diffZ))));
                                }
                            }
                        }
                    }

                }
                break;
        }
        return result;
    }

    public List<NodeSideInfo> GetNeighbours(NodeSideInfo info)
    {
        List<NodeSideInfo> result = new List<NodeSideInfo>();

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

        int minX = node.X - xDiff;
        int maxX = node.X + xDiff;
        int minY = node.Y - yDiff;
        int maxY = node.Y + yDiff;
        int minZ = node.Z - zDiff;
        int maxZ = node.Z + zDiff;

        for (int z = minZ; z <= maxZ; ++z)
        {
            for (int y = minY; y <= maxY; ++y)
            {
                for (int x = minX; x <= maxX; ++x)
                {
                    switch (info._side)
                    {
                        case Side.Back:
                        case Side.Front:
                            {
                                if ((x == minX && y == minY) || (x == maxX && y == minY) ||
                                    (x == minX && y == maxY) || (x == maxX && y == maxY))
                                {
                                    continue;
                                }
                            }
                            break;

                        case Side.Left:
                        case Side.Right:
                            {
                                if ((z == minZ && y == minY) || (z == maxZ && y == minY) ||
                                     (z == minZ && y == maxY) || (z == maxZ && y == maxY))
                                {
                                    continue;
                                }
                            }
                            break;

                        case Side.Top:
                        case Side.Bottom:
                            {
                                if ((x == minX && z == minZ) || (x == maxX && z == minZ) ||
                                     (x == minX && z == maxZ) || (x == maxX && z == maxZ))
                                {
                                    continue;
                                }
                            }
                            break;
                    }

                    if (IsInBound(x, y, z))
                    {
                        Node currentNode = Nodes[Index3D(x, y, z)];
                        if ((x == info._node.X && y == info._node.Y && z == info._node.Z)
                            || !currentNode.IsSolid)
                        {
                            continue;
                        }
                        result.Add(new NodeSideInfo(currentNode, info._side));
                    }
                }
            }
        }
        return result;
    }

    public NodeSideInfo GetNodeInfoAt(int x, int y, int z, Side side)
    {
        if (!IsInBoundRef(ref x, ref y, ref z) ||
            side >= Side.Count)
        {
            return _nodeSideInfos[Index3D(x, y, z), (int)side];;
        }

        return _nodeSideInfos[Index3D(x, y, z), (int)side];
    }


    #endregion

}
