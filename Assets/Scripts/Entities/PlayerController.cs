using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerController : MovableEntity
{
    private SpriteRenderer myRenderer;
    private HealthIndicator myHealthIndicator;

    private List<HexTile> AdjacentDestructables = new List<HexTile>();

    private bool isExtendPathButtonHeld = false;
    private bool activated = false;
    private bool isAttackingTile = false;


    protected override void OnAwake()
    {
        base.OnAwake();
        myRenderer = GetComponent<SpriteRenderer>();
        myHealthIndicator = GetComponentInChildren<HealthIndicator>();
        myHealthIndicator.EntityHasDied.AddListener(OnPlayerDeath);
        StoppingMovement.AddListener(HighlightDestructableTiles);
        StoppingMovement.AddListener(ShowNextLevelButton);
        StartWalking.AddListener(DeLightDestrucatableTiles);
        StartWalking.AddListener(HideNextLevelButton);
    }

    public override void Setup(Vector2Int startCoord, HexTile startTile)
    {
        base.Setup(startCoord, startTile);
        activated = true;
        HighlightDestructableTiles();
    }

    //Modified to account for path extending
    public override void OnPathFound(List<HexTile> path, bool succeded)
    {
        if (!succeded) { return; }

        if (isExtendPathButtonHeld)
        { oldPath.AddRange(path); }
        else
        { oldPath = path; }

        pathGizmo.SetupPath(oldPath, transform.position);
    }

    //Called by Input System
    public void InputActivateExtendSelection(InputAction.CallbackContext context)
    {
        if (!activated) { return; }
        if (context.started == false || context.canceled) { return; }
        if (isMoving) { return; }
        isExtendPathButtonHeld = true;
    }

    //Called by Input System
    public void InputDeactivateExtendSelection(InputAction.CallbackContext context)
    {
        if (!activated) { return; }
        if (context.started == true || context.canceled) { return; }
        if (isMoving) { return; }
        isExtendPathButtonHeld = false;
    }

    //Called by Input System
    //Make player character start moving along its selected path
    public void InputStartMoving(InputAction.CallbackContext context)
    {
        if (!activated) { return; }
        if (context.started == false || context.canceled) { return; }
        if (isMoving) { return; }
        if (oldPath.Count < 1) { return; }
        pathGizmo.RemovefirstPosition();
        StartCoroutine(MoveAlongPath());
        StartWalking.Invoke();
    }

    //Called by Input System
    //When a player clicks the screen, see if they select a tile
    public void InputTileSelection(InputAction.CallbackContext context)
    {
        if (!activated) { return; }
        if (!context.started || context.canceled || context.performed) { return; }
        if (isMoving) { return; }

        var mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var mousePos2D = new Vector2(mousePosition.x, mousePosition.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0f, 1 << 0);
        if (!hit) { return; }

        var hitTile = hit.transform.gameObject.GetComponent<HexTile>();
        if (!hitTile) { return; }
        ClickTile(hitTile);
    }

    //When a tile is clicked check what type of tile it is, to determine what action to take
    private void ClickTile(HexTile hitTile)
    {
        if (isAttackingTile) { return; }
        SelectionAction.Invoke();

        var isTileDestructable = HexGrid.ContainsTileTag(hitTile.tileProperties, TileTags.Destructable);
        var isTileNextToMe = HexGrid.IsTileNextTo(this.transform.position, hitTile.transform.position);

        if (isTileDestructable && isTileNextToMe && !isMoving && !isExtendPathButtonHeld)
        {
            isAttackingTile = true;
            oldPath.Clear();
            pathGizmo.SetupPath(oldPath, this.transform.position);
            StartCoroutine(AttackTile(hitTile));
        }
        else
        {
            RequestPathToTile(hitTile);
        }
    }

    //Damages a tile on a cooldown
    private IEnumerator AttackTile(HexTile hitTile)
    {
        StartWalking.Invoke();
        ShakeComponent.SetupShake(hitTile.gameObject, 1f);
        hitTile.ChangeTileColor(Color.cyan, false);//TODO find a better color
        hitTile.ReduceHealth();

        yield return new WaitForSeconds(moveTime + 0.1f);

        StoppingMovement.Invoke();
        isAttackingTile = false;
    }

    //Request a path based on selected tile and player location
    private void RequestPathToTile(HexTile hitTile)
    {
        Vector2Int currentGridPos;
        var oPCount = oldPath.Count;

        if (isExtendPathButtonHeld && oPCount > 0)
        { currentGridPos = oldPath[oPCount - 1].Coordinates; }
        else
        { currentGridPos = new Vector2Int(MyGridPos.x, MyGridPos.y); }

        var targetgridPos = new Vector2Int(hitTile.Coordinates.x, hitTile.Coordinates.y);
        PathRequestManager.RequestPath(currentGridPos, targetgridPos, false, OnPathFound, isMoving);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemy = collision.GetComponent<EnemyController>();
        var pickUp = collision.GetComponent<PickupEntity>();

        if (enemy)
        {
            myHealthIndicator.TakeDamage(myRenderer);
        }
        else if (pickUp)
        {
            ScoreIndicator.AddToScore(pickUp.CashValue);
            Destroy(pickUp.gameObject);
        }
    }

    private void OnPlayerDeath()
    {
        activated = false;
        StoppingMovement.Invoke();
        StopAllCoroutines();
        oldPath.Clear();
        pathGizmo.SetupPath(oldPath, this.transform.position);
        myHealthIndicator.ResetHealth();
        ScoreIndicator.ResetScore();
        NextLevelMenu.DisplayButton();
    }

    //Highlights the destructable tiles that are adjacent, to show that they can be clicked
    private void HighlightDestructableTiles()
    {
        if (!MyCurrentTile) { return; }
        AdjacentDestructables = HexGrid.GetAdjacentDestructableTiles(MyCurrentTile);

        ChangeLightOfDestructableTiles(true);
    }

    //Stops highlighting destructable tiles that were adjacent
    private void DeLightDestrucatableTiles()
    {
        ChangeLightOfDestructableTiles(false);
        AdjacentDestructables.Clear();
    }

    //Intermediate function used to determien if tile should lighten or darken tiles in AdjacentDestructables
    private void ChangeLightOfDestructableTiles(bool lightOrDark)
    {
        if (AdjacentDestructables.Count == 0) { return; }
        foreach (var tile in AdjacentDestructables)
        {
            tile.HighLightTile(lightOrDark);
        }
    }

    //Starts timer for sliding into display the next level button
    private void ShowNextLevelButton()
    {
        StartCoroutine(NextLevelButtonTimer());
    }

    private IEnumerator NextLevelButtonTimer()
    {
        yield return new WaitForSeconds(0.1f);
        if (!isMoving)
        {
            if (MyCurrentTile.tileProperties == TileTags.PlayerSpawn)
            {
                NextLevelMenu.DisplayButton();
            }
        }

    }

    private void HideNextLevelButton()
    {
        NextLevelMenu.HideButton();
    }
}
