using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using Foundation;

public class PathRequestManager : SingletonBase<PathRequestManager>
{
    private Queue<PathRequest> _requestQueue = new Queue<PathRequest>();
    private PathRequest _currentRequest;

    private PathFinder _finder;

    private bool _isProcessing;

    private void Awake()
    {
        _finder = GetComponent<PathFinder>();
        
    }

    public static void RequestPath(Node startPos, Node endPos, Action<Node[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(startPos, endPos, callback);
        Instance._requestQueue.Enqueue(newRequest);
        Instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!_isProcessing && _requestQueue.Count > 0)
        {
            _currentRequest = _requestQueue.Dequeue();
            _isProcessing = true;
            _finder.StartFindPath(_currentRequest._startNode, _currentRequest._endNode);
        }
    }

    public void FinishedProcessingPath(Node[] path, bool success)
    {
        _currentRequest._callback(path, success);
        _isProcessing = false;
        TryProcessNext();
    }

    public struct PathRequest
    {
        public Node _startNode;
        public Node _endNode;
        public Action<Node[], bool> _callback;

        public PathRequest(Node start, Node end, Action<Node[], bool> callback)
        {
            _startNode = start;
            _endNode = end;
            _callback = callback;
        }
    }


}
