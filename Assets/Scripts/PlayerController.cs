using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MovableEntity
{

    private bool isExtendPathButtonHeld = false;  

    public void Setup(Vector2Int startCoord)
    {
        MyGridPos = startCoord;
    }

    public override void OnPathFound(List<HexTile> path, bool succeded)
    {
        if (!succeded) { return; }

        if (isExtendPathButtonHeld)
        { oldPath.AddRange(path); }
        else
        { oldPath = path; }

        foreach (var tile in oldPath)
        {
            tile.ChangeTileColor(Color.magenta);
        }
    }

    void Update()
    {

     
        if (Input.GetMouseButtonDown(0))
        {
            if (isMoving) { return; }
            SelectionInput();
        }

        //Currently set to be left control
        if (Input.GetButton("Fire1"))
        {
            isExtendPathButtonHeld = true;
        }
        else { isExtendPathButtonHeld = false; }

        if (Input.GetButtonDown("Jump"))
        {
            if (isMoving) { return; }
            if (oldPath.Count < 1) { return; }
            StartCoroutine(MoveAlongPath());
        }
    }

    //when a player clicks the screen, see if they select a tile
    private void SelectionInput()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mousePos2D = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0f);
        if (!hit) { return; }

        var hitTile = hit.transform.gameObject.GetComponent<HexTile>();
        if (!hitTile) { return; }
        ClickTile(hitTile);
    }

    private void ClickTile(HexTile hitTile)
    {
        if (oldPath.Count > 0)
        {
            foreach (var tile in oldPath)
            {
                tile.ChangeTileColor(Color.white);
            }
        }
        Vector2Int currentGridPos;
        var oPCount = oldPath.Count;

        if (isExtendPathButtonHeld && oPCount > 0)
        { currentGridPos = oldPath[oPCount - 1].Coordinates; }
        else
        { currentGridPos = new Vector2Int(MyGridPos.x, MyGridPos.y); }

        var targetgridPos = new Vector2Int(hitTile.Coordinates.x, hitTile.Coordinates.y);
        PathRequestManager.RequestPath(currentGridPos, targetgridPos, OnPathFound);

    } 
}
