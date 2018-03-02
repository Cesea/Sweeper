using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardMovementManager))]
public class EnemyBoardObject : BoardObject
{
    public PlayerBoardObject _playerObject;

    protected BoardHealth _health;

    protected override void Awake()
    {
        base.Awake();
        _playerObject = GameObject.FindObjectOfType<PlayerBoardObject>();
        _health = GetComponent<BoardHealth>();
    }

    public override bool CheckAdjacentCells()
    {
        bool isHazardExist = false;
        foreach (var n in _adjacentCells)
        {
            if (n != null)
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

        return result;
    }
}
