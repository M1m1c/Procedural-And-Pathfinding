using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class HexGrid : MonoBehaviour
{
    static HexGrid HexGridInstance;

    public HexTile LightTilePrefab;
    public HexTile DarkTilePrefab;

    public PlayerController PlayerPrefab;

    public Text TileLabel;

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

    public static List<HexTile> GetFieldOfViewTiles(HexTile pathTile, Vector3 requesterPos)
    {
        List<HexTile> retval = new List<HexTile>();

        var adjacent = HexGridInstance.GetAdjacentTiles(pathTile);
        var adjacentDist = HexGridInstance.adjacentDist;

        for (int i = adjacent.Count - 1; i >= 0; i--)
        {

            var adjacentPos = adjacent[i].transform.position;
            var pathTilePos = pathTile.transform.position;

            var isImpassable = ((int)adjacent[i].tileProperties & 1 << (int)TileTags.Impassable) != 0;
            var isNotCloseToRequester = Vector3.Distance(requesterPos, adjacent[i].transform.position) > adjacentDist + 1f;
            var isNotCloseToPathTile =  Vector3.Distance(pathTilePos, adjacentPos) > adjacentDist + 1f;
            //var isNotCloseEnough = Mathf.Approximately( Vector3.Distance(requesterPos, adjacent[i].transform.position), adjacentDist + 1f);

            if (isImpassable || isNotCloseToRequester || isNotCloseToPathTile) 
            {
                adjacent.RemoveAt(i);
            }
        }

        if (adjacent.Count != 0) { retval = adjacent; }
        return retval;
    }

    public HexTile GetTileFromGridCoord(Vector2Int coord)
    {
        return tiles[coord.x, coord.y];
    }

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
               
                if(currentX==tempGridPos.x && currentY == tempGridPos.y) { continue; }

                if (tempGridPos.x >= width || tempGridPos.y >= height) { continue; }
                if (tempGridPos.x < 0 || tempGridPos.y < 0) { continue; }

                var foundTile = tiles[tempGridPos.x, tempGridPos.y];
                if (!foundTile) { continue; }

                var currentPos = currentTile.transform.position;
                var foundPos = foundTile.transform.position;
                if (!Mathf.Approximately(Vector3.Distance(currentPos, foundPos), adjacentDist)) { continue; }

                retval.Add(foundTile);

            }
        }
        return retval;
    }

    void Awake()
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

        //ClearGrid();
        SpawnPlayer();


        enemyMasterComp.SpawnEnemies(ref playerInstance, 2);
    }

   

    private float CalucluateAdjacentDistance()
    {
        Vector2Int tile0 = Vector2Int.zero;
        Vector2Int tile1 = new Vector2Int(1, 0);

        Vector3 pos1, pos2;

        pos1 = GetPlacementPositionFromIndex(tile0);
        pos2 = GetPlacementPositionFromIndex(tile1);

        return Vector3.Distance(pos1, pos2);
    }

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

    private Vector3 GetPlacementPositionFromIndex(Vector2Int index)
    {
        return new Vector3(index.x * (HexSettings.circumRadius * 1.5f), (index.y + index.x * 0.5f - index.x / 2) * (HexSettings.inRadius * 2.0f), 0.0f);
    }

    private HexTile GetRandomViableSpawnTile()
    {
        HexTile retval = null;
        HexTile tile = GetRandomTile();

        if (tile)
        {
            int properties = (int)tile.tileProperties;
            if ((properties & 1 << (int)TileTags.Impassable) == 0)
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

        var labels = gridCanvas.GetComponentsInChildren<Text>();

        for (int i = labels.Length - 1; i >= 0; i--)
        {
            Destroy(labels[i].gameObject);
        }
    }

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

        Text label = Instantiate(TileLabel);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.y);
        label.text = $"{tile.Coordinates.x.ToString()}\n{tile.Coordinates.y.ToString()}";

        return tile;
    }

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
