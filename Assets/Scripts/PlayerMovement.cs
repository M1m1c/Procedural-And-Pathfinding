using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Vector2Int MyGridPos { get; private set; }

    private List<HexTile> oldPath = new List<HexTile>();

    public void Setup(Vector2Int startCoord)
    {
        MyGridPos = startCoord;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectionInput();
        }
    }

    //when a player clicks the screen, see if they select a tile
    private void SelectionInput()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mousePos2D = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0f);
        if (!hit){ return; }

        var hitTile = hit.transform.gameObject.GetComponent<HexTile>();
        if (!hitTile) { return; }
        ClickTile(hitTile);
    }

    private void ClickTile(HexTile hitTile)
    {
        //TODO call find path and change color of path tiles
        if (oldPath.Count > 0)
        {
            foreach (var tile in oldPath)
            {
                tile.ChangeTileColor(Color.white);
            }
        }

        var currentGridPos = new Vector2Int(MyGridPos.x, MyGridPos.y);
        var targetgridPos = new Vector2Int(hitTile.Coordinates.x, hitTile.Coordinates.y);
        PathRequestManager.RequestPath(currentGridPos, targetgridPos, OnPathFound);
       
    }

    public void OnPathFound(List<HexTile> path, bool succeded)
    {
        if (!succeded) { return; }

       
        oldPath = path;
        foreach (var tile in path)
        {
            tile.ChangeTileColor(Color.magenta);
        }
    }
}
