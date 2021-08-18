using System.Collections.Generic;
using UnityEngine;
using System;

public class RequestPathfinding : MonoBehaviour
{
    Queue<RequestingPathQueue> requestingPathQueue = new Queue<RequestingPathQueue>();
    RequestingPathQueue _currentRequestingPathQueue;

    static RequestPathfinding instance;

    PathfindingAlgorithm _pathfindingAlgorithm;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        _pathfindingAlgorithm = GetComponent<PathfindingAlgorithm>();
    }

    public static void RequestingPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        RequestingPathQueue newQueue = new RequestingPathQueue(pathStart, pathEnd, callback);
        instance.requestingPathQueue.Enqueue(newQueue);
        instance.TryProcessNextInQueue();
    }

    void TryProcessNextInQueue()
    {
        if (!isProcessingPath && requestingPathQueue.Count > 0)
        {
            _currentRequestingPathQueue = requestingPathQueue.Dequeue();
            isProcessingPath = true;
            _pathfindingAlgorithm.StartFindPath(_currentRequestingPathQueue.pathStart, _currentRequestingPathQueue.pathEnd);
        }
    }

    public void FinishProcessingPath(Vector3[] path, bool success)
    {
        _currentRequestingPathQueue.callback(path, success);
        isProcessingPath = false;
        TryProcessNextInQueue();
    }

    struct RequestingPathQueue
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public RequestingPathQueue(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }

}
