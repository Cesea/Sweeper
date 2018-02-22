using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObject : MonoBehaviour
{
    public int MaxStamina;
    public int CurrentStamina;

    private List<BoardMoveBase> _movementComponents = new List<BoardMoveBase>();

    private Node[] _adjacentCells;
    public Node[] AdjacentCells
    {
        get { return _adjacentCells; }
    }

    private void Awake()
    {
        BoardWalk boardWalk = GetComponent<BoardWalk>();
        if (boardWalk != null)
        {
            _movementComponents.Add(boardWalk);
        }

        BoardJump boardJump = GetComponent<BoardJump>();
        if (boardJump != null)
        {
            _movementComponents.Add(boardJump);
        }

        _adjacentCells = new Node[8];
    }

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void OnMovementStart()
    {
        GameStateManager.Instance.RemoveExclamations();
    }

    public void OnMovementDone()
    {
        UpdateAdjacentCells();
        CheckAdjacentCells();

        foreach (var movement in _movementComponents)
        {
            movement.UpdatePositionInfos(transform.position);
        }
    }


    public void SetPosition(Vector2Int v)
    {
        transform.position = BoardManager.BoardPosToWorldPos(v);
        OnMovementDone();
    }

    private void CheckAdjacentCells()
    {
        bool isHazardExist = false;
        int count = 0;
        for (int z = 0; z < 3; ++z)
        {
            for (int x = 0; x < 3; ++x)
            {
                if (x == 1 && z == 1)
                {
                    continue;
                }
                Node currentCell = _adjacentCells[count];
                if (currentCell != null)
                {
                    if (currentCell.Type == Node.NodeType.Mine)
                    {
                        isHazardExist = true;
                    }
                }
                count++;
            }
        }

        if (isHazardExist)
        {
            GameStateManager.Instance.SpawnExclamation((int)transform.position.x, (int)transform.position.z);
        }
    }

    private void UpdateAdjacentCells()
    {
        GameStateManager.Instance.CurrentBoard.GetAdjacentCells((int)transform.position.x, (int)transform.position.z, ref _adjacentCells);
    }


}
