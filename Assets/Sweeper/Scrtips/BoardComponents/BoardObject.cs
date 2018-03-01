﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[RequireComponent(typeof(BoardMovementManager))]
public class BoardObject : MonoBehaviour
{
    protected BoardMovementManager _movementManager;

    protected bool _alive = true;
    public bool Alive { get { return _alive; }  set { _alive = value; } }

    [HideInInspector]
    public List<Command> _commandBuffer = new List<Command>();

    protected NodeSideInfo[] _adjacentCells;
    public NodeSideInfo[] AdjacentCells
    {
        get { return _adjacentCells; }
    }

    [SerializeField]
    protected bool _canReceiveCommand = false; 
    public bool CanReceiveCommand { get { return _canReceiveCommand; } set { _canReceiveCommand = value; } }

    protected virtual void Awake()
    {
        _movementManager = GetComponent<BoardMovementManager>();
        _adjacentCells = new NodeSideInfo[8];
    }

    public void SetSittingNode(Node node, Side side)
    {
        _movementManager.SetSittingNode(node, side);
    }

    // NOTE : if there is hazard then return true
    public virtual bool CheckAdjacentCells()
    {
        return false;
    }

    public void UpdateNeighbourCells()
    {
        _adjacentCells = BoardManager.Instance.CurrentBoard.GetNeighbours(_movementManager._sittingNodeInfo).ToArray();
    }

    public void ClearCommandBuffer(Events.RadialShutEvent e)
    {
        _commandBuffer.Clear();
    }

    public void DoCommand(int index)
    {
        if (!_canReceiveCommand && 
            index > _commandBuffer.Count - 1)
        {
            return;
        }
        _commandBuffer[index].Execute(gameObject);
        _commandBuffer.Clear();
    }

    public bool CheckMovement(NodeSideInfo sittingNodeInfo)
    {
        bool result = false;
        if (sittingNodeInfo.IsHazard)
        {
            result = true;
            _alive = false;
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
