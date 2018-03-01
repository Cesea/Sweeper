using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardMovementManager))]
public class EnemyBoardObject : BoardObject
{
    private PlayerBoardObject _playerObject;

    protected override void Awake()
    {
        base.Awake();
        _playerObject = GameObject.FindObjectOfType<PlayerBoardObject>();
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
}
