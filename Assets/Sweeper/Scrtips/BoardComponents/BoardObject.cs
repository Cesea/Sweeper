using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[RequireComponent(typeof(BoardMovementManager))]
public class BoardObject : MonoBehaviour
{
    private BoardMovementManager _movementManager;

    [HideInInspector]
    public List<Command> _commandBuffer = new List<Command>();

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

    private void OnEnable()
    {
        EventManager.Instance.AddListener<Events.RadialShutEvent>(ClearCommandBuffer);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<Events.RadialShutEvent>(ClearCommandBuffer);
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


    // NOTE : if there is hazard then return true
    public bool CheckAdjacentCells()
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

        return isHazardExist;
    }

    public void UpdateNeighbourCells(Side side)
    {
        _adjacentCells = BoardManager.Instance.CurrentBoard.GetNeighbours(_movementManager._sittingNodeInfo).ToArray();
    }

    public void ClearCommandBuffer(Events.RadialShutEvent e)
    {
        _commandBuffer.Clear();
    }

    public void DoCommand(int index)
    {
        if (index > _commandBuffer.Count - 1)
        {
            return;
        }
        _commandBuffer[index].Execute(gameObject);
        _commandBuffer.Clear();
    }
}
