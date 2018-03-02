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

    private void OnEnable()
    {
        EventManager.Instance.AddListener<Events.EnemyTurnEvent>(OnEnemyTurnEvent);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<Events.EnemyTurnEvent>(OnEnemyTurnEvent);
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

        _enemies.Add(boardObject);
        return result;
    }

    public void OnEnemyTurnEvent(Events.EnemyTurnEvent e)
    {
        StartCoroutine(EnemiesThinkAndAct());
        GameStateManager.Instance.TurnChanged = true;
    }

    IEnumerator EnemiesThinkAndAct()
    {
        foreach (var e in _enemies)
        {
            yield return null;
        }
    }
}
