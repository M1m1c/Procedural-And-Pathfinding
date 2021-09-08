using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MovableEntity
{



    public void OnPlayerWalking()
    {
        if (isMoving) { return; }
        if (oldPath.Count < 1) { return; }
        pathGizmo.RemovefirstPosition();
        StartCoroutine(MoveAlongPath());
    }
    public void OnPlayerStopping()
    {
        OnPlayerRequestingPath();
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
    }

    private void CreateNewPath()
    {
       var goalTile = HexGrid.GetRandomWalkableTileWithin(this.transform.position, 4);
        PathRequestManager.RequestPath(MyGridPos, goalTile.Coordinates, false, OnPathFound, false);
    }
}
