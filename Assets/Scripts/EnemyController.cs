using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum EnemyState
{
    None,
    Patrolling,
    FollowingPlayer
}

public class EnemyController : MovableEntity
{

    private bool isPlayerMoving = false;

    private List<HexTile> fieldOfView = new List<HexTile>();

    private HexTile facingTile;

    private MovableEntity followTarget = null;
    private Vector2Int fTargetLastCoord = new Vector2Int();

    private EnemyState myState = EnemyState.Patrolling;

    private int maxFollowSteps = 5;
    private int currentFollowSteps = 5;

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
        if (followTarget) { fTargetLastCoord = followTarget.MyGridPos; }
        if (!isPlayerMoving) { return; }
        if (isMoving) { return; }
        if (oldPath.Count < 1)
        {
            CreateNewPath();
            return;
        }
        StartCoroutine(MoveAlongPath());
    }

    public override void OnPathFound(List<HexTile> path, bool succeded)
    {
        if (!succeded ||
            (myState == EnemyState.Patrolling && path.Count > 4) ||
            (myState == EnemyState.FollowingPlayer && currentFollowSteps == 0))
        {
            followTarget = null;
            myState = EnemyState.Patrolling;
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
        if (oldPath.Count == 0) { yield return null; }
        isMoving = true;
        HexTile goalTile = null;
        while (oldPath.Count > 0)
        {

            if (oldPath.Count == 0) { break; }
            if (oldPath[0]) { facingTile = oldPath[0]; }
            UpdateFieldOfView();

            if (!isMoving) { break; }
            if (!isPlayerMoving) { break; }
            if (oldPath.Count == 0) { break; }
            var targetTile = oldPath[0];
            goalTile = targetTile;

            yield return StartCoroutine(MoveToTile(targetTile));

            pathGizmo.RemovefirstPosition();
            targetTile.DeOccupyTile(this.gameObject);
            if (oldPath.Count == 0) { break; }
            oldPath.RemoveAt(0);
        }

        if (goalTile != null)
        {
            MyGridPos = goalTile.Coordinates;
            transform.position = goalTile.transform.position;
        }

        if (myState == EnemyState.FollowingPlayer)
        {
            currentFollowSteps = Mathf.Clamp(currentFollowSteps - 1, 0, maxFollowSteps);
        }

        isMoving = false;
        yield return null;
    }

    private void CreateNewPath()
    {
        HexTile goalTile = null;

        if (myState == EnemyState.Patrolling)
        {
            while (true)
            {
                goalTile = HexGrid.GetRandomWalkableTileWithin(this.transform.position, 4);
                if (EnemyMaster.IsMoveGoalShared(goalTile, this)) { continue; }
                break;
            }
            if (!goalTile) { return; }
            PathRequestManager.RequestPath(MyGridPos, goalTile.Coordinates, false, OnPathFound, false);
        }
        else if (myState == EnemyState.FollowingPlayer)
        {
            //if (!followTarget) { return; }
            PathRequestManager.RequestPath(MyGridPos, fTargetLastCoord, false, OnPathFound, false);
        }

    }

    //Adds FOV tiles to list and Re-draws FOV by chaning color fo tiles
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

        if (newFieldOfView.Count != 0)
        {
            newFieldOfView.Add(oldPath[0]);
            foreach (var item in newFieldOfView)
            {
                item.ChangeTileColor(Color.yellow);
            }
            fieldOfView = newFieldOfView;
        }

        CheckFieldOfViewForPlayer();
    }

    private void Update()
    {
        //TODO make sure that enemy stops moving original path and starts following player as soon as player enters field of view.
        CheckFieldOfViewForPlayer();
    }

    private void CheckFieldOfViewForPlayer()
    {
        if (myState == EnemyState.FollowingPlayer) { return; }
        if (fieldOfView.Count == 0) { return; }

        foreach (var tile in fieldOfView)
        {
            if (tile.Occupants.Count == 0) { continue; }

            foreach (var occupant in tile.Occupants)
            {
                if (occupant.GetComponent<PlayerController>())
                {
                    //TODO recalculate path and pursue occupant
                    StopCoroutine(MoveAlongPath());
                    myState = EnemyState.FollowingPlayer;
                    followTarget = occupant.GetComponent<MovableEntity>();
                    fTargetLastCoord = followTarget.MyGridPos;
                    currentFollowSteps = maxFollowSteps;
                    oldPath.Clear();
                    pathGizmo.SetupPath(oldPath, this.transform.position);
                    OnPlayerRequestingPath();
                }
            }
        }
    }
}
