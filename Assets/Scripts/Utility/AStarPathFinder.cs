using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : MonoBehaviour
{
    HexGrid hexGridComp;

    void Awake()
    {
        hexGridComp = GetComponent<HexGrid>();
    }

    void FindPath(Vector2Int startPos, Vector2Int goalPos)
    {
        var startTile = hexGridComp.GetTileFromGridCoord(startPos);
        var goalTile = hexGridComp.GetTileFromGridCoord(goalPos);

        List<HexTile> availableTiles = new List<HexTile>();
        HashSet<HexTile> closedTiles = new HashSet<HexTile>();
        availableTiles.Add(startTile);

        //TODO continue working here
    }
}
