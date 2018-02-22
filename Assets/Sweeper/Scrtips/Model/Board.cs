﻿using System.Collections;
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

    Node[] _nodes;

    public int XCount { get; set; }
    public int YCount { get; set; }
    public int ZCount { get; set; }

    public int TotalCount { get { return XCount * YCount * ZCount; } }

    public Vector3 WorldStartPos;


    public Board(Vector3 boardWorldPos, Vector3 worldSize, float nodeRadius)
    {
        WorldStartPos = boardWorldPos;
        NodeRadius = nodeRadius;

        _nodeDiameter = nodeRadius * 2.0f;

        XCount = Mathf.RoundToInt(worldSize.x / _nodeDiameter);
        YCount = Mathf.RoundToInt(worldSize.y / _nodeDiameter);
        ZCount = Mathf.RoundToInt(worldSize.z / _nodeDiameter);

        _nodes = new Node[TotalCount];

        for (int z = 0; z < ZCount; ++z)
        {
            for (int y = 0; y < YCount; ++y)
            {
                for (int x = 0; x < XCount; ++x)
                {
                    int index = Index3D(x, y, z);
                    Vector3 worldPos = boardWorldPos + new Vector3(
                        x * _nodeDiameter + NodeRadius,
                        y * _nodeDiameter + NodeRadius,
                        z * _nodeDiameter + nodeRadius );

                    _nodes[index] = new Node(x, y, z, worldPos, Node.NodeType.Normal);
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
    }

    public Node GetNodeAt(int x, int y, int z)
    {
        if (!IsInBound(x, y, z))
        {
            return null;
        }
        return _nodes[Index3D(x, y, z)];
    }

    public Node GetNodeAt(Vector3 worldPos)
    {
        int indexX = (int)((worldPos.x - WorldStartPos.x) / _nodeDiameter);
        int indexY = (int)((worldPos.y - WorldStartPos.y) / _nodeDiameter);
        int indexZ = (int)((worldPos.z - WorldStartPos.z) / _nodeDiameter);

        if (!IsInBound(indexX, indexY, indexZ))
        {
            return null;
        }
        return _nodes[Index3D(indexX, indexY, indexZ)];

    }

    public Node.NodeType GetTypeAt(int x, int y, int z)
    {
        if (!IsInBound(x, y, z))
        {
            return Node.NodeType.Count;
        }
        return GetNodeAt(x, y, z).Type;
    }


    public bool CanMoveTo(int x, int y, int z)
    {
        bool result = true;
        if (!IsInBound(x, y, z))
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

    public Vector3Int GetRandomCellCoord()
    {
        return new Vector3Int((int)Random.Range(0, XCount), (int)Random.Range(0, YCount), (int)Random.Range(0, ZCount));
    }

    public bool IsInBound(int x, int y, int z)
    {
        if (x < 0 || x > XCount - 1 ||
            y < 0 || y > YCount - 1 ||
            z < 0 || z > YCount - 1)
        {
            return false;
        }
        return true;
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

                if (IsInBound(ix, y, iz))
                {
                    cells[count] = _nodes[index];
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

    private int Index3D(int x, int y, int z)
    {
        return x + (XCount * z) + (XCount * ZCount * y);
    }

    #endregion

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
