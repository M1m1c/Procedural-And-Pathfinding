using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Vector2Int MyGridPos { get; private set; }

    private List<HexTile> oldPath = new List<HexTile>();

    [SerializeField]private float moveTime = 0.6f;

    private bool isExtendPathButtonHeld = false;
    private bool isMoving = false;

    public void Setup(Vector2Int startCoord)
    {
        MyGridPos = startCoord;
    }

    public void OnPathFound(List<HexTile> path, bool succeded)
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
    private IEnumerator MoveAlongPath()
    {
        isMoving = true;
        while (oldPath.Count > 0)
        {
            var targetTile = oldPath[0];

            yield return StartCoroutine(MoveToTile(targetTile));
            targetTile.ChangeTileColor(Color.white);
            oldPath.RemoveAt(0);
        }
        isMoving = false;
        yield return null;
    }
    private IEnumerator MoveToTile(HexTile targetTile)
    {
        var elapsedTime = 0f;
        var startingPos = transform.position;
        var newPosition = targetTile.transform.position;
        while (elapsedTime < moveTime)
        {

            transform.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;

            var dist = Vector3.Distance(transform.position, newPosition);
            if (dist > -0.1f && dist < 0.1f)
            {
                var success=targetTile.OccupyTile(this.gameObject);
                if (success) { MyGridPos = targetTile.Coordinates; }
            }
            yield return null;
        }
    }
}
