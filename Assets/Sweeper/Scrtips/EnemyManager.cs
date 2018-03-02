using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class EnemyManager : SingletonBase<EnemyManager>
{
    public GameObject _enemyPrefab;

    [HideInInspector]
    public List<EnemyBoardObject> _enemies = new List<EnemyBoardObject>();

    private void Start()
    {
        
    }

    public bool SpawnEnemy(Vector3Int pos, Side side)
    {
        return SpawnEnemy(pos.x, pos.y, pos.z, side);
    }

    public bool SpawnEnemy(int x, int y, int z, Side side)
    {
        bool result = false;

        GameObject go = Instantiate(_enemyPrefab);

        go.transform.SetParent(transform);

        EnemyBoardObject boardObject = go.GetComponent<EnemyBoardObject>();
        boardObject.SetSittingNode(x, y, z, side);

        return result;
    }
}
