﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[RequireComponent(typeof(BoardMovementManager))]
public class BoardObject : MonoBehaviour
{
    protected BoardMovementManager _movementManager;
    public BoardMovementManager MovementManager { get { return _movementManager; } }

    [HideInInspector]
    private List<Command> _commandBuffer = new List<Command>();
    public List<Command> CommandBuffer { get { return _commandBuffer; } }

    protected NodeSideInfo[] _adjacentCells;
    public NodeSideInfo[] AdjacentCells { get { return _adjacentCells; } }

    public NodeSideInfo SittingNode { get { return _movementManager._sittingNodeInfo; } }

    [SerializeField]
    protected bool _canReceiveCommand = false; 
    public bool CanReceiveCommand { get { return _canReceiveCommand; } set { _canReceiveCommand = value; } }

    protected virtual void Awake()
    {
        _movementManager = GetComponent<BoardMovementManager>();
        _adjacentCells = new NodeSideInfo[8];
    }

    public void SetSittingNode(Vector3Int pos, Side side)
    {
        SetSittingNode(pos.x, pos.y, pos.z, side);
    }

    public void SetSittingNode(int x, int y, int z, Side side)
    {
        _movementManager.SetSittingNode(x, y, z, side);
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

    public virtual void DoCommand(int index)
    {
        if (!_canReceiveCommand && 
            index > _commandBuffer.Count - 1)
        {
            _commandBuffer.Clear();
            return;
        }
        _commandBuffer[index].Execute(gameObject);
        _commandBuffer.Clear();
    }

    public virtual void DoCommand(Command command)
    {
        if (!_canReceiveCommand)
        {
            return;
        }
        command.Execute(gameObject);
    }

    public virtual bool CheckMovement(NodeSideInfo sittingNodeInfo)
    {
        bool result = false;
        return result;
    }

    public virtual void OnMovementDone()
    {
    }

}
