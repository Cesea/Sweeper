using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    BoardManager _boardManager;

    public bool _find;

    public Vector3 endPos; 
    public Vector3 startPos; 

    private void Awake()
    {
        _boardManager = GetComponent<BoardManager>();
    }

    private void Update()
    {
        if (_find)
        {
            _find = false;
            Node start = _boardManager.CurrentBoard.GetNodeAt(startPos);
            Node end = _boardManager.CurrentBoard.GetNodeAt(endPos);
            FindPath(start, end);
        }
    }

    public void FindPath(Node start, Node end)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(start);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; ++i)
            {
                if (openSet[i].TotalCost < currentNode.TotalCost ||
                    openSet[i].TotalCost == currentNode.TotalCost && openSet[i].HCost < currentNode.HCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if (currentNode == end)
            {
                RetracePath(start, end);
                return;
            }

            foreach (var n in _boardManager.CurrentBoard.GetNeighbours(currentNode))
            {
                if (!n.IsSolid || closedSet.Contains(n))
                {
                    continue;
                }
                int newMovementCost = currentNode.GCost + GetDistance(currentNode, n);
                if (newMovementCost < n.GCost || !openSet.Contains(n))
                {
                    n.GCost = newMovementCost;
                    n.HCost = GetDistance(n, end);
                    n.Parent = currentNode;

                    if (!openSet.Contains(n))
                    {
                        openSet.Add(n);
                    }
                }
            }
        }
    }

    void RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.Parent;
        }
        path.Reverse();

        _boardManager.Path = path;
    }

    int GetDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.X - b.X);
        int distZ = Mathf.Abs(a.Z - b.Z);

        if (distX > distZ)
        {
            return 10 * distZ + 10 * (distX- distZ);
        }
        else
        {
            return 10 * distX + 10 * (distZ - distX);
        }
    }


}
