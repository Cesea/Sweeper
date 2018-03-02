using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardMovementManager))]
public class PlayerBoardObject : BoardObject
{
    protected BoardHealth _health;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<Events.RadialShutEvent>(ClearCommandBuffer);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<Events.RadialShutEvent>(ClearCommandBuffer);
    }

    protected override void Awake()
    {
        base.Awake();
        _health = GetComponent<BoardHealth>();
    }

    public override bool CheckAdjacentCells()
    {
        bool isHazardExist = false;
        foreach (var n in _adjacentCells)
        {
            if (!Object.ReferenceEquals(n, null))
            {
                if (n.IsHazard)
                {
                    isHazardExist = true;
                }
            }
        }
        if (isHazardExist)
        {
            GameStateManager.Instance.SpawnExclamation(transform);
        }
        return isHazardExist;
    }

    public override bool CheckMovement(NodeSideInfo sittingNodeInfo)
    {
        bool result = false;
        if (sittingNodeInfo.IsHazard)
        {
            result = true;
            //ReceiveDamage가 true를 반환했다는건 데미지가 들어갔다는 이야기 이다
            if (_health.ReceiveDamage(1))
            {
                EventManager.Instance.TriggerEvent(new Events.PlayerHealthChanged(_health.CurrentHealth)); 
            }
        }
        else if (sittingNodeInfo._node.Type == Node.NodeType.Exit)
        {
            result = true;
            GameStateManager.Instance.IsGameOver = true;
            GameStateManager.Instance.LevelFinished = true;
        }
        return result;
    }


}
