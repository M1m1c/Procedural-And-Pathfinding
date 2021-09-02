using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public HexCoordinates MyGridPos { get; set; }

    public AStarPathFinder pathFinder { get; set; }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectionInput();
        }
    }

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
        var currentGridPos = new Vector2Int(MyGridPos.X, MyGridPos.Y);
        var targetgridPos = new Vector2Int(hitTile.coordinates.X, hitTile.coordinates.Y);

        Debug.Log($"currentgridpos ={currentGridPos.x},{currentGridPos.y}");
        Debug.Log($"targetgridpos ={targetgridPos.x},{targetgridPos.y}");
        var newPath = pathFinder.FindPath(gameObject.name,currentGridPos, targetgridPos);

        foreach (var tile in newPath)
        {
            tile.ChangeTileColor(Color.magenta);
        }
    }
}
