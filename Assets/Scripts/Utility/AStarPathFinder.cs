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

    public List<HexTile> FindPath(Vector2Int startPos, Vector2Int goalPos)
    {
        var startTile = hexGridComp.GetTileFromGridCoord(startPos);
        var goalTile = hexGridComp.GetTileFromGridCoord(goalPos);

        var pathNodes = new List<PathNode>();
        var availableTiles = new List<HexTile>();
        var closedTiles = new HashSet<HexTile>();
        availableTiles.Add(startTile);
        var currentNode = new PathNode(availableTiles[0], null);

        Debug.Log("goaltile= "+goalTile.coordinates.ToString());
        while (availableTiles.Count > 0)
        {
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
                //var tempnode = new PathNode(currentTile, currentNode);
                //pathNodes.Add(tempnode);
                //currentNode = tempnode;
                return RetracePath(pathNodes, startTile, goalTile);
            }

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
                    adjacent.parent = currentTile;
                    
                    //var tempnode = new PathNode(currentTile, currentNode);
                    //pathNodes.Add(tempnode);
                    //currentNode = tempnode;

                    if (!availableTiles.Contains(adjacent)) { availableTiles.Add(adjacent); }
                }
            }
        }
        return new List<HexTile>();
    }

    //TODO look into fixing PathNode solution that is commented out below
    private List<HexTile> RetracePath(List<PathNode> pathNodes, HexTile startTile, HexTile endTile)
    {
        var path = new List<HexTile>();
        var currentTile = endTile;
       

        while (currentTile != startTile)
        {

            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Reverse();

        //PathNode currentnode = pathNodes.FirstOrDefault(item => item.MyTile == endTile);
        //if (currentnode != null)
        //{
        //    while (currentnode.MyTile != startTile)
        //    {
        //        if (currentnode.ParentNode == null)
        //        {
        //            //path = new List<HexTile>();
        //            Debug.Log("current node has no parent");
        //            break;
        //        }

        //        path.Add(currentnode.MyTile);
        //        currentnode = currentnode.ParentNode;
        //    }
        //    path.Reverse();
        //}
        //else { Debug.Log("no node to start with"); }

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
