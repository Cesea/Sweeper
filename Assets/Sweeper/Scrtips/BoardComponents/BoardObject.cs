using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObject : MonoBehaviour
{
    public int MaxStamina;
    public int CurrentStamina;

    private List<BoardMoveBase> _movementComponents = new List<BoardMoveBase>();

    private Node[] _upperCells;
    public Node[] UpperCells
    {
        get { return _upperCells; }
    }
    private Node[] _lowerCells;
    public Node[] LowerCells
    {
        get { return _lowerCells; }
    }
    private Node[] _adjacentCells;
    public Node[] AdjacentCells
    {
        get { return _adjacentCells; }
    }

    private NodeSideInfo _sittingNodeInfo;
    public NodeSideInfo SittingNodeInfo
    {
        get { return _sittingNodeInfo; }
    }

    public Side SittingSide;

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
        _upperCells = new Node[9];
        _lowerCells = new Node[9];
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

    public void OnMovementDone(NodeSideInfo info)
    {
        _sittingNodeInfo = info;
        UpdateNeighbourCells();

        //만약 플레이어가 죽거나 출구에 도착한다면 true를 반환한다
        if (GameStateManager.Instance.CheckMovement(_sittingNodeInfo._node))
        {
            return;
        }

        CheckAdjacentCells();

        foreach (var movement in _movementComponents)
        {
            movement.UpdateNodeSideInfo(_sittingNodeInfo);
        }
    }


    public void SetSittingNode(Node node, Side side)
    {
        _sittingNodeInfo = new NodeSideInfo();
        _sittingNodeInfo._node = node;
        _sittingNodeInfo._side = side;
        transform.position = _sittingNodeInfo.GetWorldPosition();
        OnMovementDone(_sittingNodeInfo);
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

    private void UpdateNeighbourCells()
    {
        //upper
        GameStateManager.Instance.CurrentBoard.GetAdjacentCellsHorizontally(
            transform.position.ToVector3Int(0, 1, 0), true, ref _upperCells);
        //center
        GameStateManager.Instance.CurrentBoard.GetAdjacentCellsHorizontally(
            transform.position.ToVector3Int(), false, ref _upperCells);
        //lower
        GameStateManager.Instance.CurrentBoard.GetAdjacentCellsHorizontally(
            transform.position.ToVector3Int(0, -1, 0), false, ref _upperCells);
    }


}
