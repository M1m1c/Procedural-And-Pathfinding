using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MovableEntity
{

    private bool isPlayerMoving = false;

    public void OnPlayerWalking()
    {
        isPlayerMoving = true;
        if (isMoving) { return; }      
        if (oldPath.Count < 1)
        {
            CreateNewPath();
            return;
        }
        //pathGizmo.RemovefirstPosition();  
        StartCoroutine(MoveAlongPath());
        
    }
    public void OnPlayerStopping()
    {
        isMoving = false;
        isPlayerMoving = false;

        if (oldPath.Count < 1)
        {
            CreateNewPath();
        }
    }

    public void OnPlayerRequestingPath()
    {

        if (oldPath.Count != 0) { return; }
        CreateNewPath();
    }

    public override void OnPathFound(List<HexTile> path, bool succeded)
    {
        if (!succeded || path.Count > 4)
        {
            CreateNewPath();
            return;
        }

        oldPath = path;
        pathGizmo.SetupPath(oldPath, transform.position);

        if (!isPlayerMoving) { return; }
        OnPlayerWalking();
    }

    protected override IEnumerator MoveAlongPath()
    {
        isMoving = true;
        HexTile goalTile = null;
        while (oldPath.Count > 0)
        {
            if (!isMoving) { break; }
            if (!isPlayerMoving) { break; }
            var targetTile = oldPath[0];
            goalTile = targetTile;
            yield return StartCoroutine(MoveToTile(targetTile));
            pathGizmo.RemovefirstPosition();

            if (oldPath.Count == 0) { break; }
            oldPath.RemoveAt(0);
        }

        if (goalTile != null)
        {
            MyGridPos = goalTile.Coordinates;
            transform.position = goalTile.transform.position;
        }
       
        isMoving = false;
        yield return null;
    }

    private void CreateNewPath()
    {
        var goalTile = HexGrid.GetRandomWalkableTileWithin(this.transform.position, 4);
        PathRequestManager.RequestPath(MyGridPos, goalTile.Coordinates, false, OnPathFound, false);
    }
}
