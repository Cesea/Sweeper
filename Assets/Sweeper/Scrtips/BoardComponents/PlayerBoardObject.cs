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
            _health.ReceiveDamage(1);
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
