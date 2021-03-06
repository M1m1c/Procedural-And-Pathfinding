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

    Heap<HexTile> openSet;

    void Awake()
    {
        hexGridComp = GetComponent<HexGrid>();
        PathRequester = GetComponent<PathRequestManager>();
        openSet = new Heap<HexTile>(hexGridComp.GridMaxSize);
    }

    public void StartFindPath(Vector2Int pathStart, Vector2Int pathEnd, bool ignoreImpassable)
    {
        StartCoroutine(FindPath(pathStart, pathEnd, ignoreImpassable));
    }

    /*
     FindPath is the major A* portion of the class.
     
     Itterates over tile neighbours between startpos and goalpos, 
     by adding them to closed set and adding/removing them from open set.

    Takes the first tile in openset (currentTile), which based on its class "Heap" is the highest priority item.
    Tiles added to open set get automatically sorted with high priority tiles ending up closer to the start of the collection.

    Compares the adjacent tiles to the picked out current tile, 
    and assigns the adjacent tiles new costs based on its distance from start and goal.
    Sets the current tile as parent of the adjacent tile.
    Adds adjacent tiles to open set if they are not already there.

    Itterates until the currenttile is the goal tile.
         */
    private IEnumerator FindPath(Vector2Int startPos, Vector2Int goalPos, bool ignoreImpassable)
    {
        var path = new List<HexTile>();
        var succeeded = false;

        var startTile = hexGridComp.GetTileFromGridCoord(startPos);
        var goalTile = hexGridComp.GetTileFromGridCoord(goalPos);

        var isGoalReachable = !HexGrid.ContainsTileTag(goalTile.tileProperties, TileTags.Impassable);
        if (isGoalReachable || ignoreImpassable)
        {
            var closedTiles = new HashSet<HexTile>();
            openSet.Add(startTile);

            while (openSet.Count > 0)
            {
                var currentTile = openSet.RemoveFirst();
                closedTiles.Add(currentTile);

                if (currentTile == goalTile)
                {
                    succeeded = true;
                    break;
                }

                var adjacentTiles = hexGridComp.GetAdjacentTiles(currentTile);
                foreach (var adjacent in adjacentTiles)
                {
                    if (!ignoreImpassable)
                    {
                        var isImpassable = HexGrid.ContainsTileTag(adjacent.tileProperties, TileTags.Impassable);
                        if (isImpassable) { continue; }
                    }

                    var newGCost = currentTile.gCost + GetGridDistanceCost(currentTile, adjacent);
                    if (closedTiles.Contains(adjacent) && newGCost >= adjacent.gCost) { continue; }

                    if (newGCost < adjacent.gCost || !openSet.Contains(adjacent))
                    {
                        adjacent.gCost = newGCost;
                        adjacent.hCost = GetGridDistanceCost(adjacent, goalTile);
                        adjacent.parent = currentTile;

                        if (!openSet.Contains(adjacent)) { openSet.Add(adjacent); }
                    }
                }
            }
        }


        yield return null;

        if (succeeded)
        {
            path = RetracePath(startTile, goalTile);
            openSet.Clear();
        }

        PathRequester.FinishedProcessingPath(path, succeeded);
    }

    //Itterates through all parents that are connected to the end tile until the start tiles is reached,
    //Then reverses the list that is gnerated and returns it
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

    //Gets a distance cost based on a gridstep cost that is modified by the coordinate distance between the parameter tiles
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
