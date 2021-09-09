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
    }

    private void CreateNewPath()
    {
       var goalTile = HexGrid.GetRandomWalkableTileWithin(this.transform.position, 4);
        PathRequestManager.RequestPath(MyGridPos, goalTile.Coordinates, false, OnPathFound, false);
    }
}
