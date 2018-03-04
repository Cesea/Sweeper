using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class EnemyManager : SingletonBase<EnemyManager>
{
    public GameObject _enemyPrefab;

    [HideInInspector]
    public List<EnemyBoardObject> _enemies = new List<EnemyBoardObject>();

    private bool _thinkFinished = false;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<Events.EnemyTurnEvent>(OnEnemyTurnEvent);

        EventManager.Instance.AddListener<Events.EnemyIdleEvent>(OnEnemyIdleEvent);
        EventManager.Instance.AddListener<Events.EnemyChaseEvent>(OnEnemyChaseEvent);
        EventManager.Instance.AddListener<Events.EnemyFleeEvent>(OnEnemyFleeEvent);
        EventManager.Instance.AddListener<Events.EnemyPatrolEvent>(OnEnemyPatrolEvent);
        EventManager.Instance.AddListener<Events.PlayerPositionEvent>(OnPlayerPositionEvent);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<Events.EnemyTurnEvent>(OnEnemyTurnEvent);

        EventManager.Instance.RemoveListener<Events.EnemyIdleEvent>(OnEnemyIdleEvent);
        EventManager.Instance.RemoveListener<Events.EnemyChaseEvent>(OnEnemyChaseEvent);
        EventManager.Instance.RemoveListener<Events.EnemyFleeEvent>(OnEnemyFleeEvent);
        EventManager.Instance.RemoveListener<Events.EnemyPatrolEvent>(OnEnemyPatrolEvent);
        EventManager.Instance.AddListener<Events.PlayerPositionEvent>(OnPlayerPositionEvent);
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
        foreach (var enemy in _enemies)
        {
            enemy._stamina.ResetStaminaToMax();
        }
        StartCoroutine(EnemiesThinkAndAct());
    }

    IEnumerator EnemiesThinkAndAct()
    {
        foreach (var e in _enemies)
        {
            e.Think();
            yield return new WaitForSecondsRealtime(1.0f);
        }
        GameStateManager.Instance.TurnChanged = true;
    }

    public void OnEnemyIdleEvent(Events.EnemyIdleEvent e)
    {
    }
    public void OnEnemyChaseEvent(Events.EnemyChaseEvent e)
    {
        EnemyBoardObject target = null;
        foreach (var enemy in _enemies)
        {
            if (enemy.EnemyID == e.EnemyID)
            {
                target = enemy;
            }
        }
        VisualEffectManager.Instance.SpawnExclamation(target);
    }
    public void OnEnemyFleeEvent(Events.EnemyFleeEvent e)
    {
    }
    public void OnEnemyPatrolEvent(Events.EnemyPatrolEvent e)
    {
    }

    public void OnPlayerPositionEvent(Events.PlayerPositionEvent e)
    {
        foreach (var enemy in _enemies)
        {
            enemy.NotifyPlayerPosition(e.SittingInfo);
        }
    }
}
