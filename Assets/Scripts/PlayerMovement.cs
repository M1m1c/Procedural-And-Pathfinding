using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public HexCoordinates MyGridPos { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

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
        var camPos = Camera.main.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(camPos, mousePosition - camPos);
        if (!hit){ return; }

        var hitTile = hit.transform.gameObject.GetComponent<HexTile>();
        if (!hitTile) { return; }
        ClickTile(hitTile);
    }

    private void ClickTile(HexTile hitTile)
    {

    }
}
