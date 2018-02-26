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

    public static void RequestPath(NodeSideInfo startInfo, NodeSideInfo endInfo, Action<NodeSideInfo[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(startInfo, endInfo, callback);
        Instance._requestQueue.Enqueue(newRequest);
        Instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!_isProcessing && _requestQueue.Count > 0)
        {
            _currentRequest = _requestQueue.Dequeue();
            _isProcessing = true;
            _finder.StartFindPath(_currentRequest._startNodeInfo, _currentRequest._endNodeInfo);
        }
    }

    public void FinishedProcessingPath(NodeSideInfo[] path, bool success)
    {
        _currentRequest._callback(path, success);
        _isProcessing = false;
        TryProcessNext();
    }

    public struct PathRequest
    {
        public NodeSideInfo _startNodeInfo;
        public NodeSideInfo _endNodeInfo;
        public Action<NodeSideInfo[], bool> _callback;

        public PathRequest(NodeSideInfo start, NodeSideInfo end, Action<NodeSideInfo[], bool> callback)
        {
            _startNodeInfo = start;
            _endNodeInfo = end;
            _callback = callback;
        }
    }


}
