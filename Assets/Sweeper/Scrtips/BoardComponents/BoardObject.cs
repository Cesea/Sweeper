using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardMovementManager))]
public class BoardObject : MonoBehaviour
{
    private BoardMovementManager _movementManager;

    private NodeSideInfo[] _adjacentCells;
    public NodeSideInfo[] AdjacentCells
    {
        get { return _adjacentCells; }
    }

    private void Awake()
    {
        _movementManager = GetComponent<BoardMovementManager>();

        _adjacentCells = new NodeSideInfo[8];
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

    public void CheckAdjacentCells()
    {
        bool isHazardExist = false;
        foreach (var n in _adjacentCells)
        {
            if (n != null)
            {
                Node currentNode = n._node;
                if (currentNode.IsHazard)
                {
                    isHazardExist = true;
                }
            }
        }

        if (isHazardExist)
        {
            GameStateManager.Instance.SpawnExclamation((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        }
    }

    public void UpdateNeighbourCells(Side side)
    {
        _adjacentCells = BoardManager.Instance.CurrentBoard.GetNeighbours(_movementManager._sittingNodeInfo).ToArray();
    }


}
