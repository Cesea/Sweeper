using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell 
{
    public enum CellType
    {
        Empty,
        Mine,
        Start,
        Exit,
        Count
    }

    private CellType _type;
    private int _x;
    private int _z;

    public int X
    {
        get { return _x; }
        set { _x = value; }
    }
    public int Z
    {
        get { return _z; }
        set { _z = value; }
    }

    public CellType Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public Cell(int x, int z, CellType type)
    {
        _x = x;
        _z = z;
        _type = type;
    }

    
}
