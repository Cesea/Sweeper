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
        Exit
    }

    private CellType _type;
    private int _x;
    private int _y;

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

    public CellType Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public Cell(int x, int y, CellType type)
    {
        _x = x;
        _y = y;
        _type = type;
    }

    
}
