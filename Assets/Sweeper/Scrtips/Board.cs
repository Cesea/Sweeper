using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [Header ("Cube Prefabs")]
    [SerializeField]
    GameObject _emptyPrefab;
    [SerializeField]
    GameObject _minePrefab;
    [SerializeField]
    GameObject _startPrefab;
    [SerializeField]
    GameObject _exitPrefab;


    [Header("Board Size")]
    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;

    Cell[] _cells;

	void Start ()
    {
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

        _cells[0].Type = Cell.CellType.Start;
        _cells[6 + 6 * _width].Type = Cell.CellType.Exit;

        for (int z = 0; z < _height; ++z)
        {
            for (int x = 0; x < _width; ++x)
            {
                GameObject prefab = GetPrefabByType(_cells[x + _width * z].Type);
                GameObject go = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.identity);
                go.name = "cube_" + x + "_" + z;
                go.transform.SetParent(transform);
            }
        }
	}

    public Cell.CellType GetTypeAt(int x, int z)
    {
        return _cells[x + _width * z].Type;
    }

    private GameObject GetPrefabByType(Cell.CellType type)
    {
        GameObject result = null;
        switch (type)
        {
            case Cell.CellType.Empty:
                {
                    result = _emptyPrefab;
                }
                break;
            case Cell.CellType.Mine :
                {
                    result = _minePrefab;
                } break;
            case Cell.CellType.Start :
                {
                    result = _startPrefab;
                } break;
            case Cell.CellType.Exit :
                {
                    result = _exitPrefab;
                } break;
        }
        return result;
    }

    public bool CanMoveTo(int x, int z)
    {
        bool result = true;
        if (x < 0 || z < 0 || x > _width - 1 || z > _height - 1)
        {
            result = false;
        }
        return result;
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
