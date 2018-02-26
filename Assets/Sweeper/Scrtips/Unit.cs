using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform targetTransform;

    float _speed = 1;
    Node[] _path;
    private Node _target;
    int _targetIndex = 0;

	void Start ()
    {
        Vector3 pos = transform.position;
        pos.y -= 1.0f;
        Node start = BoardManager.Instance.CurrentBoard.GetNodeAt(pos);
        Node end = BoardManager.Instance.CurrentBoard.GetNodeAt(targetTransform.position);

        PathRequestManager.RequestPath(start, end, OnPathFind);
	}

    public void OnPathFind(Node[] nodes, bool success)
    {
        _path = nodes;
        if (success)
        {
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Node currentNode = _path[0];
        while (true)
        {
            if (transform.position == currentNode.GetWorldPositionBySide(Side.Top))
            {
                _targetIndex++;
                if (_targetIndex >= _path.Length)
                {
                    yield break;
                }
                currentNode = _path[_targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentNode.GetWorldPositionBySide(Side.Top), _speed * Time.deltaTime);
            yield return null;
        }
    }
}
