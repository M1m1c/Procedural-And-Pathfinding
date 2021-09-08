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

    public static void RequestPath(Vector2Int pathStart, Vector2Int pathEnd,bool ignoreImpassable,Action<List<HexTile>,bool> callBack, bool isAlreadyMoving)
    {
        if (isAlreadyMoving) { return; }
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, ignoreImpassable, callBack);
        PathRequesterInstance.PathRequestQueue.Enqueue(newRequest);
        PathRequesterInstance.TryProcessNextRequest();
    }

    private void TryProcessNextRequest()
    {
        if (!isProceesingPath && PathRequestQueue.Count > 0)
        {
            currentRequest = PathRequestQueue.Dequeue();
            isProceesingPath = true;

            PathFinder.StartFindPath(currentRequest.pathStart, currentRequest.pathEnd, currentRequest.ignoreImpassable);
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
        public bool ignoreImpassable;
        public Action<List<HexTile>, bool> callBack;

        public PathRequest(Vector2Int start, Vector2Int end,bool ignore, Action<List<HexTile>, bool> call)
        {
            pathStart = start;
            pathEnd = end;
            ignoreImpassable = ignore;
            callBack = call;
        }
    }
}
