using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardMovementManager))]
public class EnemyBoardObject : BoardObject
{
    public BoardHealth _health;
    public BoardStamina _stamina;

    protected AI.AIThinker _thinker;

    public PlayerBoardObject _playerObject;

    protected override void Awake()
    {
        base.Awake();
        _health = GetComponent<BoardHealth>();
        _stamina = GetComponent<BoardStamina>();
        _thinker = GetComponent<AI.AIThinker>();

        _playerObject = FindObjectOfType<PlayerBoardObject>();
    }

    public virtual void Think()
    {
        _thinker.Think();
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

        return result;
    }
}
