using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Board
{
    private Vector2Int _startCell;
    private Vector2Int _exitCell;

    public Vector2Int StartCellCoord
    {
        get { return _startCell; }
        set { _startCell = value; }
    }
    public Vector2Int ExitCellCoord
    {
        get { return _exitCell; }
        set { _exitCell = value; }
    }

    public Vector2 WorldSize;
    public float NodeRadius;

    private float _nodeDiameter;

    Node[] _nodes;
    private int _xCount;
    private int _yCount;

    public int XCount { get; set; }
    public int YCount { get; set; }
    public int TotalCount { get { return XCount * YCount; } }

    public Vector3 WorldStartPos;



    public Board(Vector3 boardWorldPos, Vector2 worldSize, float nodeRadius)
    {
        WorldStartPos = boardWorldPos;
        NodeRadius = nodeRadius;
        _nodeDiameter = nodeRadius * 2.0f;

        XCount = Mathf.RoundToInt(worldSize.x / _nodeDiameter);
        YCount = Mathf.RoundToInt(worldSize.y / _nodeDiameter);

        _nodes = new Node[XCount * YCount];

        for (int y = 0; y < YCount; ++y)
        {
            for (int x = 0; x < XCount; ++x)
            {
                int index = x + XCount * y;
                Vector3 worldPos = boardWorldPos + new Vector3(x * _nodeDiameter + NodeRadius, 0, y * _nodeDiameter + NodeRadius);
                _nodes[index] = new Node(x, y, worldPos, Node.NodeType.Normal);

                float randomValue = Random.Range(0.0f, 1.0f);
                if (randomValue > 0.80f && randomValue < 0.9f)
                {
                    _nodes[index].Type = Node.NodeType.Empty;
                }
                else if (randomValue > 0.9f && randomValue < 1.0f)
                {
                    _nodes[index].Type = Node.NodeType.Wall;
                }
            }
        }

        _startCell = GetRandomCellCoord();
        _exitCell = GetRandomCellCoord();
        if (_startCell == _exitCell)
        {
            while (_startCell == _exitCell)
            {
                _exitCell = GetRandomCellCoord();
            }
        }

        _nodes[_startCell.x + _startCell.y * XCount].Type = Node.NodeType.Start;
        _nodes[_exitCell.x + _exitCell.y * XCount].Type = Node.NodeType.Exit;

        SetAdjacentCells(_startCell.x, _startCell.y, Node.NodeType.Normal);
    }


    public Node GetNodeAt(int x, int z)
    {
        if (!IsInBound(x, z))
        {
            return null;
        }
        return _nodes[x + XCount * z];
    }

    public Node GetNodeAt(Vector3 worldPos)
    {
        int indexX = (int)((worldPos.x - WorldStartPos.x) / _nodeDiameter);
        int indexY = (int)((worldPos.z - WorldStartPos.z) / _nodeDiameter);

        if (!IsInBound(indexX, indexY))
        {
            return null;
        }
        return _nodes[indexX + XCount * indexY];

    }

    public Node.NodeType GetTypeAt(int x, int z)
    {
        if (!IsInBound(x, z))
        {
            return Node.NodeType.Count;
        }
        return _nodes[x + XCount * z].Type;
    }


    public bool CanMoveTo(int x, int z)
    {
        bool result = true;
        if (!IsInBound(x, z))
        {
            result = false;
            Debug.Log("movement out of range");
        }
        return result;
    }

    public void DestoryAllLevelObjects()
    {
        foreach (var n in _nodes)
        {
            if (n.InstalledObject != null)
            {
                GameObject.Destroy(n.InstalledObject);    
            }
        }
    }

    public Vector2Int GetRandomCellCoord()
    {
        return new Vector2Int((int)Random.Range(0, XCount), (int)Random.Range(0, YCount));
    }

    public bool IsInBound(int x, int z)
    {
        if (x < 0 || z < 0 || x > XCount - 1 || z > YCount - 1)
        {
            return false;
        }
        return true;
    }

    public void GetAdjacentCells(int x, int z, ref Node[] cells)
    {
        int count = 0;
        for (int iz = z - 1; iz <= z + 1; ++iz)
        {
            for (int ix = x - 1; ix <= x + 1; ++ix)
            {
                int index = ix + XCount * iz;
                if (ix == x && iz == z)
                {
                    cells[count] = null;
                    continue;
                }
                if (IsInBound(ix, iz))
                {
                    cells[count] = _nodes[index];
                }
                count++;
            }
        }
    }

    private void SetAdjacentCells(int x, int z, Node.NodeType type)
    {
        for (int iz = z - 1; iz <= z + 1; ++iz)
        {
            for (int ix = x - 1; ix <= x + 1; ++ix)
            {
                int index = ix + XCount * iz;
                if (ix == x && iz == z)
                {
                    continue;
                }
                if (IsInBound(ix, iz))
                {
                    _nodes[index].Type = type;
                }
            }
        }
    }

    //public int GetAdjacentSum(int x, int z)
    //{
    //    int minX = x - 1;
    //    int minZ = z - 1;
    //    int maxX = x + 1;
    //    int maxZ = z + 1;
    //    minX = Mathf.Clamp(minX, 0, _width - 1);
    //    minZ = Mathf.Clamp(minZ, 0, _height - 1);
    //    maxX = Mathf.Clamp(maxX, 0, _width - 1);
    //    maxZ = Mathf.Clamp(maxZ, 0, _height - 1);
    //    int result = 0;
    //    for (int iz = minZ; iz <= maxZ; ++iz)
    //    {
    //        for (int ix = minX; ix <= maxX; ++ix)
    //        {
    //            if (ix == x && iz == z)
    //            {
    //                continue;
    //            }
    //            if (GetValueAt(ix, iz))
    //            {
    //                result += 1;
    //            }
    //        }
    //    }
    //    return result;
    //} 

}
