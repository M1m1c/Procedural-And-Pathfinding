using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarPathFinder : MonoBehaviour
{
    [SerializeField] private int gridStepCost = 10;

    HexGrid hexGridComp;

    PathRequestManager PathRequester;

    void Awake()
    {
        hexGridComp = GetComponent<HexGrid>();
        PathRequester = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector2Int pathStart, Vector2Int pathEnd)
    {
        StartCoroutine(FindPath(pathStart, pathEnd));
    }

    private IEnumerator FindPath( Vector2Int startPos, Vector2Int goalPos)
    {
        var path = new List<HexTile>();
        var succeeded = false;

        var startTile = hexGridComp.GetTileFromGridCoord(startPos);
        var goalTile = hexGridComp.GetTileFromGridCoord(goalPos);

        var isGoalReacable = ((int)goalTile.tileProperties & 1 << (int)TileTags.Impassable) == 0;
        if (isGoalReacable)
        {
            var availableTiles = new List<HexTile>();//new Heap<HexTile>(hexGridComp.GridMaxSize);
            //var availableTiles = new Heap<HexTile>(hexGridComp.GridMaxSize);
            var closedTiles = new HashSet<HexTile>();
            availableTiles.Add(startTile);

            while (availableTiles.Count > 0)
            {
                //var currentTile = availableTiles.RemoveFirst();
                var currentTile = availableTiles[0];
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

                if (currentTile == goalTile)
                {
                    succeeded = true;
                    break;
                }

                var adjacentTiles = hexGridComp.GetAdjacentTiles(currentTile);
                foreach (var adjacent in adjacentTiles)
                {
                    var isImpassable = ((int)adjacent.tileProperties & 1 << (int)TileTags.Impassable) != 0;
                    if (isImpassable) { continue; }

                    var tentative_gCost = currentTile.gCost + GetGridDistanceCost(currentTile, adjacent);
                    if (closedTiles.Contains(adjacent) && tentative_gCost >= adjacent.gCost) { continue; }
                    
                    if (tentative_gCost < adjacent.gCost || !availableTiles.Contains(adjacent))
                    {
                        adjacent.gCost = tentative_gCost;
                        adjacent.hCost = GetGridDistanceCost(adjacent, goalTile);
                        adjacent.parent = currentTile;

                        if (!availableTiles.Contains(adjacent)) { availableTiles.Add(adjacent); }
                    }
                }
            }
        }

       
        yield return null;

        if (succeeded)
        {
            path = RetracePath(startTile, goalTile);
        }

        PathRequester.FinishedProcessingPath(path, succeeded);
    }

  

    //TODO look into fixing PathNode solution that is commented out below
    private List<HexTile> RetracePath( HexTile startTile, HexTile endTile)
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
        var distX = Mathf.Abs(tileA.Coordinates.x - tileB.Coordinates.x);
        var distY = Mathf.Abs(tileA.Coordinates.y - tileB.Coordinates.y);

        if (distX > distY)
        { retval = (distY * gridStepCost) + gridStepCost * (distX - distY); }
        else
        { retval = (distX * gridStepCost) + gridStepCost * (distY - distX); }

        return retval;
    }
}
