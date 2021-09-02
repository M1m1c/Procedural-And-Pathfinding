using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarPathFinder : MonoBehaviour
{
    [SerializeField] private int gridStepCost = 10;

    HexGrid hexGridComp;

    void Awake()
    {
        hexGridComp = GetComponent<HexGrid>();
    }

    public List<HexTile> FindPath(string entityID, Vector2Int startPos, Vector2Int goalPos)
    {
        var startTile = hexGridComp.GetTileFromGridCoord(startPos);
        var goalTile = hexGridComp.GetTileFromGridCoord(goalPos);

        var availableTiles = new List<HexTile>();
        var closedTiles = new HashSet<HexTile>();
        availableTiles.Add(startTile);


         Debug.Log("goaltile= "+goalTile.coordinates.ToString());
        while (availableTiles.Count > 0)
        {
            var currentTile = availableTiles[0];

            PathNode currentTileNode;
            if (!currentTile.TilePathNode.ContainsKey(entityID))
            {
                currentTile.TilePathNode.Add(entityID, new PathNode());
            }
            currentTileNode = currentTile.TilePathNode[entityID];

            for (int i = 0; i < availableTiles.Count; i++)
            {
                var indexTile = availableTiles[i];
                var indexTileNode = indexTile.TilePathNode[entityID];
                if (indexTileNode.fCost <= currentTileNode.fCost && indexTileNode.hCost < currentTileNode.hCost) 
                {
                    currentTile = indexTile;
                }
            }
            availableTiles.Remove(currentTile);
            closedTiles.Add(currentTile);


            if (currentTile == goalTile)    { return RetracePath(entityID, startTile, goalTile); }


            var adjacentTiles = hexGridComp.GetAdjacentTiles(currentTile);
            foreach (var adjacentTile in adjacentTiles)
            {
                PathNode adjacentTileNode;
                if (!adjacentTile.TilePathNode.ContainsKey(entityID))
                {
                    adjacentTile.TilePathNode.Add(entityID, new PathNode());
                }
                adjacentTileNode = adjacentTile.TilePathNode[entityID];

                var isImpassable = ((int)adjacentTile.tileProperties & 1 << (int)TileTags.Impassable) != 0;
                if (isImpassable || closedTiles.Contains(adjacentTile)){ continue; }

                var newMoveCostToAdjacent = currentTileNode.gCost + GetGridDistanceCost(currentTile, adjacentTile);
                if (newMoveCostToAdjacent < adjacentTile.TilePathNode[entityID].gCost || !availableTiles.Contains(adjacentTile)) 
                {

                    adjacentTileNode.gCost = newMoveCostToAdjacent;
                    adjacentTileNode.hCost = GetGridDistanceCost(adjacentTile,goalTile);
                    adjacentTileNode.parent = currentTile;

                    if (!availableTiles.Contains(adjacentTile)) { availableTiles.Add(adjacentTile); }
                }
            }
        }
        return new List<HexTile>();
    }

    //TODO look into fixing PathNode solution that is commented out below
    private List<HexTile> RetracePath(string entityID, HexTile startTile, HexTile endTile)
    {
        var path = new List<HexTile>();
        var currentTile = endTile;


        while (currentTile != startTile)
        {

            path.Add(currentTile);
            var tempPointer = currentTile;
            currentTile = currentTile.TilePathNode[entityID].parent;
            tempPointer.TilePathNode.Remove(entityID);
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
