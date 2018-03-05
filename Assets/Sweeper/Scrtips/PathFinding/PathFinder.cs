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

    public void StartFindPath(NodeSideInfo startNode, NodeSideInfo endNode)
    {
        StartCoroutine(FindPathRoutine(startNode, endNode));
    }

    public bool FindPathImmediate(NodeSideInfo startNode, NodeSideInfo endNode, ref NodeSideInfo[] outInfos)
    {
        if (CalculatePath(startNode, endNode))
        {
            outInfos = RetracePath(startNode, endNode);
            return true;
        }
        return false;
    }

    IEnumerator FindPathRoutine(NodeSideInfo start, NodeSideInfo end)
    {
        bool findSuccess = CalculatePath(start, end);
        NodeSideInfo[] wayPoints = new NodeSideInfo[0];
        {
            yield return null;
            if (findSuccess)
            {
                wayPoints = RetracePath(start, end);
            }
            _requestManager.FinishedProcessingPath(wayPoints, findSuccess);
        }
    }

    private bool CalculatePath(NodeSideInfo start, NodeSideInfo end)
    {
        bool findSuccess = false;
        if (start._node.IsSolid && end._node.IsSolid)
        {
            List<NodeSideInfo> openSet = new List<NodeSideInfo>();
            HashSet<NodeSideInfo> closedSet = new HashSet<NodeSideInfo>();

            openSet.Add(start);
            while (openSet.Count > 0)
            {
                NodeSideInfo currentNodeInfo = openSet[0];
                for (int i = 1; i < openSet.Count; ++i)
                {
                    if (openSet[i].TotalCost < currentNodeInfo.TotalCost ||
                        openSet[i].TotalCost == currentNodeInfo.TotalCost && openSet[i].HCost < currentNodeInfo.HCost)
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

                foreach (var n in _boardManager.CurrentBoard.GetReachables(currentNodeInfo))
                {
                    if ((!n._node.IsSolid) ||
                        (!n.IsPassable) ||
                        (n._side == Side.Count) ||
                        (closedSet.Contains(n)))
                    {
                        continue;
                    }

                    if (n.GetNodeAtSide().IsSolid)
                    {
                        continue;
                    }

                    int newMovementCost = currentNodeInfo.GCost + 10;
                    if (newMovementCost < n.GCost || !openSet.Contains(n))
                    {
                        n.GCost = newMovementCost;
                        n.HCost = GetDistanceCost(n, end);
                        n.Parent = currentNodeInfo;

                        if (!openSet.Contains(n))
                        {
                            openSet.Add(n);
                        }
                    }
                }
            }
        }
        return findSuccess;
    }

    NodeSideInfo[] RetracePath(NodeSideInfo start, NodeSideInfo end)
    {
        List<NodeSideInfo> path = new List<NodeSideInfo>();
        NodeSideInfo current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.Parent;
        }
        path.Reverse();

        return path.ToArray();
    }

    private int GetDistanceCost(NodeSideInfo a, NodeSideInfo b)
    {
        int distX = Mathf.Abs(a._node.X - b._node.X);
        int distY = Mathf.Abs(a._node.Y - b._node.Y);
        int distZ = Mathf.Abs(a._node.Z - b._node.Z);

        return distX * 10 + distY * 10 + distZ * 10;
    }


}
