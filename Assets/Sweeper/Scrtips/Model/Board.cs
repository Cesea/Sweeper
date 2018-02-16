using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Board 
{
    private Vector2Int _startCell;
    private Vector2Int _exitCell;

    public Vector2Int StartCell
    {
        get { return _startCell; }
        set { _startCell = value; }
    }
    public Vector2Int ExitCell
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

        _cells[_startCell.x + _startCell.y * _width].Type = Cell.CellType.Start;
        _cells[_exitCell.x + _exitCell.y * _width].Type = Cell.CellType.Exit;
    }

    public Cell GetCellAt(int x, int z)
    {
        return _cells[x + _width * z];
    }

    public Cell.CellType GetTypeAt(int x, int z)
    {
        return _cells[x + _width * z].Type;
    }


    public bool CanMoveTo(int x, int z)
    {
        bool result = true;
        if (x < 0 || z < 0 || x > _width - 1 || z > _height - 1)
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

    public Cell[] GetAdjacentCells(int x, int z)
    {

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
