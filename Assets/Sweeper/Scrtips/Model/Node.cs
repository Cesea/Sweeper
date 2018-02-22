using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{

    public GameObject InstalledObject { get; set; }
    public GameObject SittingObject { get; set; }

    public enum Side
    {
        Left, 
        Right,
        Top,
        Bottom,
        Front,
        Back
    }

    public enum NodeType
    {
        Empty,
        Normal,
        Start,
        Exit,
        Wall,
        Count
    }

    private NodeType _type;
    private int _x;
    private int _y;
    private int _z;

    public bool IsHazard { get; set; }
    public bool IsPassable { get; set; }

    public Vector3 WorldPosition;

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
                    } break;
                case NodeType.Normal:
                    {
                        IsHazard = false;
                        IsPassable = true;
                    } break;
                case NodeType.Start:
                    {
                        IsHazard = false;
                        IsPassable = true;
                    } break;
                case NodeType.Exit:
                    {
                        IsHazard = false;
                        IsPassable = true;
                    } break;
                case NodeType.Wall:
                    {
                        IsHazard = false;
                        IsPassable = false;
                    } break;
            }
        }
    }

    public Node(int x, int y, int z, Vector3 worldPos, NodeType type)
    {
        _x = x;
        _y = y;
        _z = z;
        _type = type;
        WorldPosition = worldPos;
    }

    
}
