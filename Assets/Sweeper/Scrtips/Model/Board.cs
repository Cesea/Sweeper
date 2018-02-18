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

    private int _width; 
    private int _height; 
    public int Width
    {
        get { return _width; }
        set { _width = value; }
    }
    public int Height
    {
        get { return _height; }
        set { _height = value; }
    }

    Cell[] _cells;

    public Board(int width, int height)
    {
        _width = width;
        _height = height;
        _cells = new Cell[_width * _height];
        for (int z = 0; z < _height; ++z)
        {
            for (int x = 0; x < _width; ++x)
            {
                int index = x + _width * z;
                _cells[index] = new Cell(x, z, Cell.CellType.Empty);
                if (Random.Range(0.0f, 1.0f) > 0.85f)
                {
                    _cells[index].Type = Cell.CellType.Mine;
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

        _cells[_startCell.x + _startCell.y * _width].Type = Cell.CellType.Start;
        _cells[_exitCell.x + _exitCell.y * _width].Type = Cell.CellType.Exit;

        SetAdjacentCells(_startCell.x, _startCell.y, Cell.CellType.Empty);
    }


    public Cell GetCellAt(int x, int z)
    {
        if (!IsInBound(x, z))
        {
            return null;
        }
        return _cells[x + _width * z];
    }

    public Cell.CellType GetTypeAt(int x, int z)
    {
        if (!IsInBound(x, z))
        {
            return Cell.CellType.Count;
        }
        return _cells[x + _width * z].Type;
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

    public Vector2Int GetRandomCellCoord()
    {
        return new Vector2Int((int)Random.Range(0, _width), (int)Random.Range(0, _height));
    }

    public bool IsInBound(int x, int z)
    {
        if (x < 0 || z < 0 || x > _width - 1 || z > _height - 1)
        {
            return false;
        }
        return true;
    }

    public void GetAdjacentCells(int x, int z, ref Cell[] cells)
    {
        int count = 0;
        for (int iz = z - 1; iz <= z + 1; ++iz)
        {
            for (int ix = x - 1; ix <= x + 1; ++ix)
            {
                int index = ix + _width * iz;
                if (ix == x && iz == z)
                {
                    cells[count] = null;
                    continue;
                }
                if (IsInBound(ix, iz))
                {
                    cells[count] = _cells[index];
                }
                count++;
            }
        }
    }

    private void SetAdjacentCells(int x, int z, Cell.CellType type)
    {
        for (int iz = z - 1; iz <= z + 1; ++iz)
        {
            for (int ix = x - 1; ix <= x + 1; ++ix)
            {
                int index = ix + _width * iz;
                if (ix == x && iz == z)
                {
                    continue;
                }
                if (IsInBound(ix, iz))
                {
                    _cells[index].Type = type;
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
