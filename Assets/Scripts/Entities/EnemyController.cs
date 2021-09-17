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
    public SpriteRenderer AlertedSymbol;

    private bool isPlayerMoving = false;

    private List<HexTile> fieldOfView = new List<HexTile>();

    private HexTile facingTile;

    private MovableEntity followTarget = null;
    private Vector2Int fTargetLastCoord = new Vector2Int();

    private EnemyState myState = EnemyState.Patrolling;

    private int maxFollowSteps = 5;
    private int currentFollowSteps = 5;

    protected override void OnAwake()
    {
        base.OnAwake();
        if (AlertedSymbol) { AlertedSymbol.enabled = false; }
    }

    //called via player event
    public void OnPlayerStartWalking()
    {
        isPlayerMoving = true;
        OnPlayerStillMoving();
    }

    //called via player event
    public void OnPlayerStopping()
    {
        isPlayerMoving = false;  
        OnPlayerSelectionAction();
    }


    //called via player event
    public void OnPlayerSelectionAction()
    {

        if (oldPath.Count != 0) { return; }
        CreateNewPath();
    }


    //called via player event
    public void OnPlayerStillMoving()
    {
        if (!isPlayerMoving) { return; }
        if (isMoving) { return; }
        if (oldPath.Count < 1)
        {
            CreateNewPath();
            return;
        }
        StartCoroutine(MoveAlongPath());
    }

    //Modified to account for enemy movement limitations and
    //so that if enemy does not find a path it will try again, otherwise enemies would get stuck.
    public override void OnPathFound(List<HexTile> path, bool succeded)
    {
        isRequestingPath = false;

        if (!succeded ||
            (myState == EnemyState.Patrolling && path.Count > 4) ||
            (myState == EnemyState.FollowingPlayer && currentFollowSteps == 0))
        {
            followTarget = null;
            AlertedSymbol.enabled = false;
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

    //Enemy's MoveAlongPath() requires multiple null checks for path,
    //since if they spot player their path might be cleared before they exit the loop.
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
            var oldTile = MyCurrentTile;
            var targetTile = oldPath[0];
            goalTile = targetTile;

            yield return StartCoroutine(MoveToTile(targetTile));

            pathGizmo.RemovefirstPosition();
            oldTile.DeOccupyTile(this.gameObject);
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
    
    //Requests a path from PathRequestManager
    private void CreateNewPath()
    {
        HexTile goalTile = null;
        if (isRequestingPath) { return; }
        isRequestingPath = true;    

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
            PathRequestManager.RequestPath(MyGridPos, fTargetLastCoord, false, OnPathFound, false);
        }

    }

    //Adds FOV tiles to list and Re-draws FOV by changing color of tiles
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
                item.ChangeToDefaultColor();
            }
        }

        if (newFieldOfView.Count != 0)
        {
            newFieldOfView.Add(oldPath[0]);
            foreach (var item in newFieldOfView)
            {
                item.ChangeTileColor(Color.yellow,false);
            }
            fieldOfView = newFieldOfView;
        }

        CheckFieldOfViewForPlayer();
    }

    private void Update()
    {
        if (myState == EnemyState.FollowingPlayer && followTarget) { fTargetLastCoord = followTarget.MyGridPos; }
        CheckFieldOfViewForPlayer();
    }

    //Intermediate fucntions that calls different funcitons depending on what state the enemy is in
    private void CheckFieldOfViewForPlayer()
    {
        if (fieldOfView.Count == 0) { return; }
        if (myState == EnemyState.FollowingPlayer)
        {
            GoThroughFOVTiles(ContinueFollowingPlayer);
        }
        else if (myState == EnemyState.Patrolling)
        {
            GoThroughFOVTiles(StartFollowingPlayer);
        }      
    }

    //Checks the current FOV tiles to see if the playe is inside them, calls action if player is found
    private void GoThroughFOVTiles(Action<GameObject> actionToPerfomrIfplayerFound)
    {
        foreach (var tile in fieldOfView)
        {
            if (!tile) { continue; }
            if (tile.Occupants.Count == 0) { continue; }

            foreach (var occupant in tile.Occupants)
            {
                if (!occupant) { continue; }
                if (occupant.GetComponent<PlayerController>())
                {
                    actionToPerfomrIfplayerFound(occupant);
                }
            }
        }
    }

    private void StartFollowingPlayer(GameObject occupant)
    {
        StopCoroutine(MoveAlongPath());
        ShakeComponent.SetupShake(this.gameObject, 1f);
        AlertedSymbol.enabled = true;
        myState = EnemyState.FollowingPlayer;
        followTarget = occupant.GetComponent<MovableEntity>();
        fTargetLastCoord = followTarget.MyGridPos;
        currentFollowSteps = maxFollowSteps;
        oldPath.Clear();
        pathGizmo.SetupPath(oldPath, this.transform.position);
        OnPlayerSelectionAction();
    }

    private void ContinueFollowingPlayer(GameObject occupant)
    {
        currentFollowSteps = maxFollowSteps;
    }
}
