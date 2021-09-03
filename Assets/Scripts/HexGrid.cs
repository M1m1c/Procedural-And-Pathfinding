using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class HexGrid : MonoBehaviour
{

   
    public HexTile LightTilePrefab;
    public HexTile DarkTilePrefab;

    public PlayerMovement PlayerPrefab;

    public Text TileLabel;

    private Dictionary<int, HexTile> tileSet = new Dictionary<int, HexTile>();

    private Canvas gridCanvas;

    [SerializeField] private int height = 8;
    [SerializeField] private int width = 16;
    public int GridMaxSize { get { return width * height; } }

    [SerializeField] private float scale = 10.0f;
    [SerializeField] private float offsetX = 10.0f;
    [SerializeField] private float offsetY = 10.0f;

    private float adjacentDist = 0.0f;

    HexTile[,] tiles;

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

                if (tempGridPos.x >= tiles.GetLength(0) || tempGridPos.y >= tiles.GetLength(1)) { continue; }
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
        gridCanvas = GetComponentInChildren<Canvas>();

        tileSet.Add(0, LightTilePrefab);
        tileSet.Add(1, DarkTilePrefab);

        tiles = new HexTile[width, height];

        adjacentDist = CalucluateAdjacentDistance();

        GenerateGrid();

        //ClearGrid();
        SpawnPlayer();
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

    private Vector3 GetPlacementPositionFromIndex(Vector2Int index)
    {
        return new Vector3(index.x * (HexSettings.circumRadius * 1.5f), (index.y + index.x * 0.5f - index.x / 2) * (HexSettings.inRadius * 2.0f), 0.0f);
    }

    private void SpawnPlayer()
    {
        if (!PlayerPrefab) { return; }
        while (true)
        {
            var x = UnityEngine.Random.Range(0, width);
            var y = UnityEngine.Random.Range(0, height);

            var tile = tiles[x, y];
            if (!tile) { continue; }
           
            int properties = (int)tile.tileProperties;
            if ((properties & 1 << (int)TileTags.Impassable) != 0) { continue; }

            var playerInstance = Instantiate(PlayerPrefab);

            playerInstance.transform.position = tile.transform.position;
            playerInstance.Setup(tile.Coordinates);
            tile.OccupyTile(playerInstance.gameObject);
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
                    //var flooredPerlin = Mathf.FloorToInt(clampedPerlin * tileSet.Count);
                    //tiles.Add(CreateTile(x, y,tileSet[flooredPerlin]));
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
