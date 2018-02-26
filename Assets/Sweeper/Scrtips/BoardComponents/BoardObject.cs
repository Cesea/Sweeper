using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardMovementManager))]
public class BoardObject : MonoBehaviour
{
    private BoardMovementManager _movementManager;

    private Node[] _adjacentCells;
    public Node[] AdjacentCells
    {
        get { return _adjacentCells; }
    }

    private void Awake()
    {
        _movementManager = GetComponent<BoardMovementManager>();

        _adjacentCells = new Node[8];
    }

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void SetSittingNode(Node node, Side side)
    {
        _movementManager.SetSittingNode(node, side);
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
                    if (currentCell.IsHazard)
                    {
                        isHazardExist = true;
                    }
                }
                count++;
            }
        }

        if (isHazardExist)
        {
            GameStateManager.Instance.SpawnExclamation((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        }
    }

    private void UpdateNeighbourCells(Side side)
    {
        //center
        GameStateManager.Instance.CurrentBoard.GetAdjacentCellsHorizontally(
            transform.position.ToVector3Int(), false, ref _upperCells);
        //lower
    }


}
