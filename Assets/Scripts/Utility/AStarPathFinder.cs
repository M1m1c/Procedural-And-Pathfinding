using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : MonoBehaviour
{
    [SerializeField] private int gridStepCost = 10;

    HexGrid hexGridComp;

    void Awake()
    {
        hexGridComp = GetComponent<HexGrid>();
    }

    public List<HexTile> FindPath(Vector2Int startPos, Vector2Int goalPos)
    {
        var startTile = hexGridComp.GetTileFromGridCoord(startPos);
        var goalTile = hexGridComp.GetTileFromGridCoord(goalPos);

        List<HexTile> availableTiles = new List<HexTile>();
        HashSet<HexTile> closedTiles = new HashSet<HexTile>();
        availableTiles.Add(startTile);

        while (availableTiles.Count > 0)
        {
            HexTile currentTile = availableTiles[0];
            for (int i = 0; i < availableTiles.Count; i++)
            {
                var indexTile = availableTiles[i];
                if (indexTile.fCost <= currentTile.fCost && indexTile.hCost < currentTile.hCost) 
                {
                    currentTile = indexTile;
                }
            }
            availableTiles.Remove(currentTile);
            closedTiles.Add(currentTile);

            if (currentTile == goalTile) { return RetracePath(startTile, goalTile); }

            var adjacentTiles = hexGridComp.GetAdjacentTiles(currentTile);
            foreach (var adjacent in adjacentTiles)
            {
                var isImpassable = ((int)adjacent.tileProperties & 1 << (int)TileTags.Impassable) != 0;
                if (isImpassable || closedTiles.Contains(adjacent)){ continue; }

                var newMoveCostToAdjacent = currentTile.gCost + GetGridDistanceCost(currentTile, adjacent);
                if (newMoveCostToAdjacent < adjacent.gCost || !availableTiles.Contains(adjacent)) 
                {
                    adjacent.gCost = newMoveCostToAdjacent;
                    adjacent.hCost = GetGridDistanceCost(adjacent,goalTile);

                    //TODO Don't have parent be on individual tiles, because only one entity can use the pathfinding system in that case.
                    adjacent.parent = currentTile;

                    if (!availableTiles.Contains(adjacent)) { availableTiles.Add(adjacent); }
                }
            }
        }
        return new List<HexTile>();
    }

    private List<HexTile> RetracePath(HexTile startTile, HexTile endTile)
    {
        var path = new List<HexTile>();
        var currentTile = endTile;

        while (currentTile != startTile) 
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Reverse();
        return path;
    }

    private int GetGridDistanceCost(HexTile tileA, HexTile tileB)
    {
        var retval = 0;
        var distX = Mathf.Abs(tileA.coordinates.X - tileB.coordinates.X);
        var distY = Mathf.Abs(tileA.coordinates.Y - tileB.coordinates.Y);

        if (distX > distY) { retval = gridStepCost * (distX - distY); }
        else { retval = gridStepCost * (distY - distX); }

        return retval;
    }
}
