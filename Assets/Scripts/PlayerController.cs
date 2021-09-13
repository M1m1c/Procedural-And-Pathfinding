using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerController : MovableEntity
{ 

    private bool isExtendPathButtonHeld = false;

    private SpriteRenderer myRenderer;
    private HealthIndicator myHealthIndicator;

    private bool activated = true;


    protected override void OnAwake()
    {
        base.OnAwake();
        myRenderer = GetComponent<SpriteRenderer>();
        myHealthIndicator = GetComponentInChildren<HealthIndicator>();
        myHealthIndicator.EntityHasDied.AddListener(OnPlayerDeath);
        
    }

    public override void OnPathFound(List<HexTile> path, bool succeded)
    {
        if (!succeded) { return; }

        if (isExtendPathButtonHeld)
        { oldPath.AddRange(path); }
        else
        { oldPath = path; }

        pathGizmo.SetupPath(oldPath, transform.position);
    }

    public void InputActivateExtendSelection(InputAction.CallbackContext context)
    {
        if (!activated) { return; }
        if (context.started == false || context.canceled) { return; }
        if (isMoving) { return; }
        isExtendPathButtonHeld = true;
    }

    public void InputDeactivateExtendSelection(InputAction.CallbackContext context)
    {
        if (!activated) { return; }
        if (context.started == true || context.canceled) { return; }
        if (isMoving) { return; }
        isExtendPathButtonHeld = false;
    }

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

    //when a player clicks the screen, see if they select a tile
    public void InputTileSelection(InputAction.CallbackContext context)
    {
        if (!activated) { return; }
        if (!context.started || context.canceled || context.performed) { return; }
        if (isMoving) { return; }

        var mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var mousePos2D = new Vector2(mousePosition.x, mousePosition.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0f);
        if (!hit) { return; }

        var hitTile = hit.transform.gameObject.GetComponent<HexTile>();
        if (!hitTile) { return; }
        ClickTile(hitTile);
    }

    private void ClickTile(HexTile hitTile)
    {
        Vector2Int currentGridPos;
        var oPCount = oldPath.Count;

        if (isExtendPathButtonHeld && oPCount > 0)
        { currentGridPos = oldPath[oPCount - 1].Coordinates; }
        else
        { currentGridPos = new Vector2Int(MyGridPos.x, MyGridPos.y); }

        var targetgridPos = new Vector2Int(hitTile.Coordinates.x, hitTile.Coordinates.y);
        PathRequestManager.RequestPath(currentGridPos, targetgridPos, false, OnPathFound, isMoving);
        RequestingPath.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyController>())
        {
            myHealthIndicator.TakeDamage(myRenderer);
        }
    }

    private void OnPlayerDeath()
    {
        activated = false;
    }
}
