using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class HexGrid : MonoBehaviour
{
    static HexGrid HexGridInstance;

    public HexTile LightTilePrefab;
    public HexTile DarkTilePrefab;

    public PlayerController PlayerPrefab;

    public PickupEntity TreassuePrefab;

    public Text TileLabel;

    public Sprite ExitTileSprite;

    private PlayerController playerInstance;

    private Dictionary<int, HexTile> tileSet = new Dictionary<int, HexTile>();

    private Canvas gridCanvas;

    private EnemyMaster enemyMasterComp;

    [SerializeField] private int height = 8;
    [SerializeField] private int width = 16;
    public int GridMaxSize { get { return width * height; } }

    [SerializeField] private float scale = 10.0f;
    [SerializeField] private float offsetX = 10.0f;
    [SerializeField] private float offsetY = 10.0f;

    private float adjacentDist = 0.0f;

    HexTile[,] tiles;

    private HexTile playerSpawnPoint;

    private bool isDebugging = false;

    private void Awake()
    {
        HexGridInstance = this;
        gridCanvas = GetComponentInChildren<Canvas>();
        enemyMasterComp = GetComponent<EnemyMaster>();

        tileSet.Add(0, LightTilePrefab);
        tileSet.Add(1, DarkTilePrefab);

        tiles = new HexTile[width, height];

        adjacentDist = CalucluateAdjacentDistance();

        GenerateGrid();

        RemoveUnreachableTiles();

        SpawnPlayer();

        enemyMasterComp.SpawnEnemies(ref playerInstance, 3);

        SpawnMultipleTreassures();
    }

    //Used to get a tile that is walkable and within the tile count
    public static HexTile GetRandomWalkableTileWithin(Vector3 requsterPos, int tileCount)
    {
        HexTile retval = null;

        while (true)
        {
            var tile = HexGridInstance.GetRandomViableSpawnTile();
            if (!tile) { continue; }

            var b = HexGridInstance.IsTileWithinDistanceSpan(requsterPos, tile.transform.position, tileCount, true);
            if (!b) { continue; }
            retval = tile;
            break;

        }

        return retval;
    }

    //Gets a tile that is walkable and not the player spawn or too close to it
    public static HexTile GetRandomEnemySpawn()
    {
        var tile = HexGridInstance.GetRandomViableSpawnTile();
        if (!tile) { return null; }

        if (tile == HexGridInstance.playerSpawnPoint) { return null; }

        var tilePos = tile.transform.position;
        var playerPos = HexGridInstance.playerSpawnPoint.transform.position;

        if (HexGridInstance.IsTileWithinDistanceSpan(tilePos, playerPos, 3, true)) { return null; }
        return tile;
    }

    //Gets the tiles to the sides of the next pathTile and the tile currently standing on.
    public static List<HexTile> GetFieldOfViewTiles(HexTile pathTile, Vector3 requesterPos)
    {
        List<HexTile> retval = new List<HexTile>();

        var adjacent = HexGridInstance.GetAdjacentTiles(pathTile);
        var adjacentDist = HexGridInstance.adjacentDist;

        for (int i = adjacent.Count - 1; i >= 0; i--)
        {

            var adjacentPos = adjacent[i].transform.position;
            var pathTilePos = pathTile.transform.position;

            var isImpassable = ContainsTileTag(adjacent[i].tileProperties, TileTags.Impassable);
            var isNotCloseToRequester = Vector3.Distance(requesterPos, adjacent[i].transform.position) > adjacentDist + 1f;
            var isNotCloseToPathTile =  Vector3.Distance(pathTilePos, adjacentPos) > adjacentDist + 1f;

            if (isImpassable || isNotCloseToRequester || isNotCloseToPathTile) 
            {
                adjacent.RemoveAt(i);
            }
        }

        if (adjacent.Count != 0) { retval = adjacent; }
        return retval;
    }

    public static List<HexTile> GetAdjacentDestructableTiles(HexTile currentTile)
    {
        List<HexTile> retval = HexGridInstance.GetAdjacentTiles(currentTile);

        retval.RemoveAll(q => !ContainsTileTag(q.tileProperties, TileTags.Destructable) ||
        !HexGridInstance.AreTilesAdjacent(currentTile.transform.position, q.transform.position));

        return retval;
    }

    public static bool IsTileNextTo(Vector3 posA, Vector3 posB)
    {
        return HexGridInstance.AreTilesAdjacent(posA, posB);
    }

    public static bool ContainsTileTag(TileTags objecTag, TileTags tagToCheck)
    {
        return ((int)objecTag & 1 << (int)tagToCheck) != 0;
    }

    public HexTile GetTileFromGridCoord(Vector2Int coord)
    {
        return tiles[coord.x, coord.y];
    }

    //Checks a nine tile section based on current tile,
    //and removes tiles whose distance is greater than the adjacent distance.
    public List<HexTile> GetAdjacentTiles(HexTile currentTile)
    {
        var retval = new List<HexTile>();

        var currentX = currentTile.Coordinates.x;
        var currentY = currentTile.Coordinates.y;
        var tempGridPos = new Vector2Int();


        for (int i = -1; i <= 1; i++)
        {  
            tempGridPos.x = currentX + i;

            for (int q = -1; q <= 1; q++)
            {
                tempGridPos.y = currentY + q;

                if (currentX == tempGridPos.x && currentY == tempGridPos.y) { continue; }

                if (tempGridPos.x >= width || tempGridPos.y >= height) { continue; }
                if (tempGridPos.x < 0 || tempGridPos.y < 0) { continue; }

                var foundTile = tiles[tempGridPos.x, tempGridPos.y];
                if (!foundTile) { continue; }

                var currentPos = currentTile.transform.position;
                var foundPos = foundTile.transform.position;
                if (!AreTilesAdjacent(currentPos, foundPos)) { continue; }

                retval.Add(foundTile);

            }
        }
        return retval;
    }
   
    //Calculates the distance between two abstract tiles that are known to be adjacent and returs that value.
    private float CalucluateAdjacentDistance()
    {
        Vector2Int tile0 = Vector2Int.zero;
        Vector2Int tile1 = new Vector2Int(1, 0);

        Vector3 pos1, pos2;

        pos1 = GetPlacementPositionFromIndex(tile0);
        pos2 = GetPlacementPositionFromIndex(tile1);

        return Vector3.Distance(pos1, pos2);
    }

    private bool AreTilesAdjacent(Vector3 posA, Vector3 posB)
    {
        return Mathf.Approximately(Vector3.Distance(posA, posB), adjacentDist);
    }

    //Used to check if two tiles are within a tile distance span, either fewer than tile count or more than.
    private bool IsTileWithinDistanceSpan(Vector3 posA, Vector3 posB, int tileCount, bool lessOrMoreThanCount)
    {
        var retval = false;

        var tiledistance = adjacentDist * tileCount;

        if (lessOrMoreThanCount)
        {
            if (Vector3.Distance(posA, posB) < tiledistance) { retval = true; }
        }
        else
        {
            if (Vector3.Distance(posA, posB) > tiledistance) { retval = true; }
        }

        return retval;
    }

    //Returns the worldspace position that the tile should be placed at to line up correctly within the columns and rows.
    //Needed because the tile is hexagonal,
    //and therefore the tiles cannot be placed lined up next to eachother like if they were squares.
    //see for more info regarding the math:https://www.omnicalculator.com/math/hexagon
    private Vector3 GetPlacementPositionFromIndex(Vector2Int index)
    {
        return new Vector3(index.x * (HexSettings.circumRadius * 1.5f), (index.y + index.x * 0.5f - index.x / 2) * (HexSettings.inRadius * 2.0f), 0.0f);
    }

    // returns a random tile that is walkable
    private HexTile GetRandomViableSpawnTile()
    {
        HexTile retval = null;
        HexTile tile = GetRandomTile();

        if (tile)
        {
            if (!ContainsTileTag(tile.tileProperties, TileTags.Impassable)) 
            {
                retval = tile;
            }
        }
        
        return retval;

    }

    private HexTile GetRandomTile()
    {
        var x = UnityEngine.Random.Range(0, width);
        var y = UnityEngine.Random.Range(0, height);
        return tiles[x, y];
    }

    //Spawns and sets up player and playerspawn/exit tile
    private void SpawnPlayer()
    {
        if (!PlayerPrefab) { return; }
        while (true)
        {
            var tile = GetRandomViableSpawnTile();
            if (!tile) { continue; }

            var player = Instantiate(PlayerPrefab);

            player.transform.position = tile.transform.position;
            player.Setup(tile.Coordinates,tile);
            playerSpawnPoint = tile;
            tile.OccupyTile(player.gameObject);
            playerInstance = player;

            playerSpawnPoint.tileProperties = TileTags.PlayerSpawn;
            playerSpawnPoint.ChangeSprite(ExitTileSprite);
            playerSpawnPoint.ChangeTileColor(new Color(0.5699003f, 0.9528302f, 0.6225688f,1f),true);
            break;
        }
    }

    private void SpawnMultipleTreassures()
    {
        if (!TreassuePrefab) { return; }
        var rand = Random.Range(3, 9);

        for (int i = 0; i < rand; i++)
        {
            SpawnTreassure();
        }
    }

    private void SpawnTreassure()
    {
        while (true)
        {
            var tile = GetRandomViableSpawnTile();
            if (!tile) { continue; }

            if (tile == playerSpawnPoint) { continue; }
            var treassure = Instantiate(TreassuePrefab);

            treassure.transform.position = tile.transform.position;

            break;
        }
    }


    private void ClearGrid()
    {

        foreach (var item in tiles)
        {
            if(item)
            Destroy(item.gameObject);
        }

        if (isDebugging)
        {
            var labels = gridCanvas.GetComponentsInChildren<Text>();

            for (int i = labels.Length - 1; i >= 0; i--)
            {
                Destroy(labels[i].gameObject);
            }
        }      
    }

    //Removes tiles that have no adjacent tiles
    private void RemoveUnreachableTiles()
    {
        foreach (var tile in tiles)
        {
            if (!tile) { continue; }

            if (GetAdjacentTiles(tile).Count == 0)
            {
                Destroy(tile.gameObject);
            }
        }
    }

    //Creates a grid of tiles based on width and height.
    //Uses perlin noise to determine what type of tile to spawn at the grid coordinate.
    private void GenerateGrid()
    {
        offsetX= UnityEngine.Random.Range(0.0f, 99999.0f);
        offsetY= UnityEngine.Random.Range(0.0f, 99999.0f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float rawPerlin = Mathf.PerlinNoise(
                    (float)x / width * scale + offsetX,
                    (float)y / height * scale + offsetY);
                var clampedPerlin = Mathf.Clamp(rawPerlin, 0.0f, 1.0f);

              
                if(clampedPerlin > 0.3f && clampedPerlin < 0.4f)
                {
                    tiles[x, y] = CreateTile(x, y, tileSet[1]);
                }
                else if (clampedPerlin > 0.3f)
                {
                    tiles[x, y] = CreateTile(x, y, tileSet[0]);
                }


            }
        }
    }

    private HexTile CreateTile(int x, int y, HexTile tilePrefab)
    {
        var coordPos = new Vector2Int(x, y);
        Vector3 position = GetPlacementPositionFromIndex(coordPos);

        HexTile tile = Instantiate(tilePrefab);
        tile.Setup(coordPos);
        tile.transform.SetParent(transform, false);
        tile.transform.localPosition = position;

        if (isDebugging)
        {
            Text label = Instantiate(TileLabel);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.y);
            label.text = $"{tile.Coordinates.x.ToString()}\n{tile.Coordinates.y.ToString()}";
        }
        return tile;
    }

    //Returns the world position of tile based on grid coordinates and if it exists
    private Vector3 GetTileWorldPos(Vector2Int tileCoordinate, ref bool didFindTile)
    {
        Vector3 retval = Vector3.zero;
        didFindTile = false;

        var foundTile = tiles[tileCoordinate.x, tileCoordinate.y];
        if (foundTile)
        {
            didFindTile = true;
            retval = foundTile.transform.position;
        }

        return retval;
    }
}
