using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MovableEntity
{

    private bool isPlayerMoving = false;

    private List<HexTile> fieldOfView = new List<HexTile>();

    private HexTile facingTile;

    public void OnPlayerStartWalking()
    {
        isPlayerMoving = true;
        OnPlayerStillMoving();
    }
    public void OnPlayerStopping()
    {
        isMoving = false;
        isPlayerMoving = false;
        OnPlayerRequestingPath();
    }

    public void OnPlayerRequestingPath()
    {

        if (oldPath.Count != 0) { return; }
        CreateNewPath();
    }

    public void OnPlayerStillMoving()
    {
        if (!isPlayerMoving) { return; }
        if (isMoving) { return; }
        if (oldPath.Count < 1)
        {
            CreateNewPath();
            return;
        }
        //pathGizmo.RemovefirstPosition();  
        StartCoroutine(MoveAlongPath());
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
        
        if (oldPath.Count > 0)
        {
            facingTile = oldPath[0];
            UpdateFieldOfView();
        }
       
       
        if (!isPlayerMoving) { return; }
        OnPlayerStartWalking();
    }

    protected override IEnumerator MoveAlongPath()
    {
        isMoving = true;
        HexTile goalTile = null;
        while (oldPath.Count > 0)
        {
            if (oldPath[0]) { facingTile = oldPath[0]; }
            UpdateFieldOfView();

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
        HexTile goalTile = null;
        while (true)
        {
            goalTile= HexGrid.GetRandomWalkableTileWithin(this.transform.position, 4);
            if (EnemyMaster.IsMoveGoalShared(goalTile, this)) { continue; }
            break;
        }

        if (!goalTile) { return; }
        PathRequestManager.RequestPath(MyGridPos, goalTile.Coordinates, false, OnPathFound, false);
    }

    private void UpdateFieldOfView()
    {
        var lookTile = facingTile;
        if (oldPath.Count == 0) { return; }  
        if (!lookTile) { lookTile = oldPath[0]; }
        var newFieldOfView = HexGrid.GetFieldOfViewTiles(lookTile, this.transform.position);
        
        if (fieldOfView.Count != 0)
        {
            foreach (var item in fieldOfView)
            {
                item.ChangeTileColor(Color.white);
            }
        }
         
        if(newFieldOfView.Count != 0)
        {
            newFieldOfView.Add(oldPath[0]);
            foreach (var item in newFieldOfView)
            {
                item.ChangeTileColor(Color.yellow);
            }
            fieldOfView = newFieldOfView;
        }
       
    }
}
