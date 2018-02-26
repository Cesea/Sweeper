using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private BoardManager _boardManager;
    private PathRequestManager _requestManager;

    private void Awake()
    {
        _boardManager = GetComponent<BoardManager>();
        _requestManager = GetComponent<PathRequestManager>();
    }

    private void Update()
    {
    }

    public void StartFindPath(NodeSideInfo startNode, NodeSideInfo endNode)
    {
        StartCoroutine(FindPath(startNode, endNode));
    }

    IEnumerator FindPath(NodeSideInfo start, NodeSideInfo end)
    {
        if (start._node.IsSolid && end._node.IsSolid)
        {
            NodeSideInfo[] wayPoints = new NodeSideInfo[0];
            bool findSuccess = false;

            List<NodeSideInfo> openSet = new List<NodeSideInfo>();
            HashSet<NodeSideInfo> closedSet = new HashSet<NodeSideInfo>();

            openSet.Add(start);
            while (openSet.Count > 0)
            {
                NodeSideInfo currentNodeInfo = openSet[0];
                for (int i = 1; i < openSet.Count; ++i)
                {
                    if (openSet[i]._node.TotalCost < currentNodeInfo._node.TotalCost ||
                        openSet[i]._node.TotalCost == currentNodeInfo._node.TotalCost && openSet[i]._node.HCost < currentNodeInfo._node.HCost)
                    {
                        currentNodeInfo = openSet[i];
                    }
                }
                openSet.Remove(currentNodeInfo);
                closedSet.Add(currentNodeInfo);

                if (currentNodeInfo == end)
                {
                    findSuccess = true;
                    break;
                }

                foreach (var n in _boardManager.CurrentBoard.GetNeighbours(currentNodeInfo))
                {
                    if (!n._node.IsSolid || closedSet.Contains(n))
                    {
                        continue;
                    }

                    if (n.GetNodeAtSide().IsSolid)
                    {
                        continue;
                    }

                    int newMovementCost = currentNodeInfo._node.GCost + GetDistance(currentNodeInfo._node, n._node);
                    if (newMovementCost < n._node.GCost || !openSet.Contains(n))
                    {
                        n._node.GCost = newMovementCost;
                        n._node.HCost = GetDistance(n._node, end._node);
                        n._node.Parent = currentNodeInfo._node;

                        if (!openSet.Contains(n))
                        {
                            openSet.Add(n);
                        }
                    }
                }
            }
            yield return null;
            if (findSuccess)
            {
                wayPoints = RetracePath(start._node, end._node);
            }
            _requestManager.FinishedProcessingPath(wayPoints, findSuccess);
        }
    }

    NodeSideInfo[] RetracePath(Node start, Node end)
    {
        List<NodeSideInfo> path = new List<NodeSideInfo>();
        Node current = end;

        while (current != start)
        {
            path.Add(new NodeSideInfo(current, Side.Top));
            current = current.Parent;
        }
        path.Reverse();

        return path.ToArray();
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
