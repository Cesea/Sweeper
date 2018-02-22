using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public GameObject InstalledObject { get; set; }
    public GameObject SittingObject { get; set; }

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
    private int _z;

    public bool IsHazard { get; set; }
    public bool IsWalkable { get; set; }

    public Vector3 WorldPosition;

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
                        IsWalkable = true;
                    } break;
                case NodeType.Normal:
                    {
                        IsHazard = false;
                        IsWalkable = true;
                    } break;
                case NodeType.Start:
                    {
                        IsHazard = false;
                        IsWalkable = true;
                    } break;
                case NodeType.Exit:
                    {
                        IsHazard = false;
                        IsWalkable = true;
                    } break;
                case NodeType.Wall:
                    {
                        IsHazard = false;
                        IsWalkable = false;
                    } break;
            }
        }
    }

    public Node(int x, int z, Vector3 worldPos, NodeType type)
    {
        _x = x;
        _z = z;
        _type = type;
        WorldPosition = worldPos;
    }

    
}
