using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> PathRequestQueue = new Queue<PathRequest>();
    PathRequest currentRequest;

    static PathRequestManager PathRequesterInstance;
    AStarPathFinder PathFinder;

    bool isProceesingPath = false;

    private void Awake()
    {
        PathRequesterInstance = this;
        PathFinder = GetComponent<AStarPathFinder>();
    }

    public static void RequestPath(Vector2Int pathStart, Vector2Int pathEnd,Action<List<HexTile>,bool> callBack)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callBack);
        PathRequesterInstance.PathRequestQueue.Enqueue(newRequest);
        PathRequesterInstance.TryProcessNextRequest();
    }

    private void TryProcessNextRequest()
    {
        if (!isProceesingPath && PathRequestQueue.Count > 0)
        {
            currentRequest = PathRequestQueue.Dequeue();
            isProceesingPath = true;

            PathFinder.StartFindPath(currentRequest.pathStart, currentRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(List<HexTile> path, bool wasSucessfull)
    {
        currentRequest.callBack(path, wasSucessfull);
        isProceesingPath = false;
        PathRequesterInstance.TryProcessNextRequest();
    }

    struct PathRequest
    {
        public Vector2Int pathStart;
        public Vector2Int pathEnd;
        public Action<List<HexTile>, bool> callBack;

        public PathRequest(Vector2Int start, Vector2Int end, Action<List<HexTile>, bool> call)
        {
            pathStart = start;
            pathEnd = end;
            callBack = call;
        }
    }
}
