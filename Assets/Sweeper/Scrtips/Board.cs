using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField]
    GameObject _cubePrefab;

    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;

    bool[] _cells;

	void Start ()
    {
        _cells = new bool[_width * _height];
        for (int z = 0; z < _height; ++z)
        {
            for (int x = 0; x < _width; ++x)
            {
                int index = x + _width * z;
                if (Random.Range(0.0f, 1.0f) > 0.8f)
                {
                    _cells[index] = true;
                }
                else
                {
                    _cells[index] = false;
                }
            }
        }


        for (int z = 0; z < _height; ++z)
        {
            for (int x = 0; x < _width; ++x)
            {
                GameObject go = Instantiate(_cubePrefab, new Vector3(x, 0, z), Quaternion.identity);
                go.name = "cube_" + x + "_" + z;
                go.transform.SetParent(transform);
                if (!GetValueAt(x, z))
                {
                    Text text = go.GetComponentInChildren<Text>();
                    if (text != null)
                    {
                        text.text = GetAdjacentSum(x, z).ToString();
                    }
                }
            }
        }
	}

    public bool GetValueAt(int x, int z)
    {
        return _cells[x + _width * z];
    }

    public int GetAdjacentSum(int x, int z)
    {
        int minX = x - 1;
        int minZ = z - 1;
        int maxX = x + 1;
        int maxZ = z + 1;
        minX = Mathf.Clamp(minX, 0, _width - 1);
        minZ = Mathf.Clamp(minZ, 0, _height - 1);
        maxX = Mathf.Clamp(maxX, 0, _width - 1);
        maxZ = Mathf.Clamp(maxZ, 0, _height - 1);

        int result = 0;

        for (int iz = minZ; iz <= maxZ; ++iz)
        {
            for (int ix = minX; ix <= maxX; ++ix)
            {
                if (ix == x && iz == z)
                {
                    continue;
                }
                if (GetValueAt(ix, iz))
                {
                    result += 1;
                }
            }
        }

        return result;

    } 

}
