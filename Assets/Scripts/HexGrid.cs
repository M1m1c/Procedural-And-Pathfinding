using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HexGrid : MonoBehaviour
{

   
    public HexTile lightTilePrefab;
    public HexTile darkTilePrefab;

    public PlayerMovement playerPrefab;

    public Text tileLabel;

    private Dictionary<int, HexTile> tileSet = new Dictionary<int, HexTile>();

    private Canvas gridCanvas;

    [SerializeField] private int height = 8;
    [SerializeField] private int width = 16;

    [SerializeField] private float scale = 10.0f;
    [SerializeField] private float offsetX = 10.0f;
    [SerializeField] private float offsetY = 10.0f;

    HexTile[,] tiles;

    public HexTile GetTileFromGridCoord(Vector2Int coord)
    {
        return tiles[coord.x, coord.y];
    }

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        tileSet.Add(0, lightTilePrefab);
        tileSet.Add(1, darkTilePrefab);

        tiles = new HexTile[width, height];

        GenerateGrid();

        //TODO create clear grid function
        //TODO create button for clearing and generating a new grid
        //TODO Add new different colored tiles

        //ClearGrid();
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (!playerPrefab) { return; }
        while (true)
        {
            var x = Random.Range(0, width);
            var y = Random.Range(0, height);

            var tile = tiles[x, y];
            if (!tile) { continue; }

            int properties = (int)tile.tileProperties;
            if ((properties & 1 << (int)TileTags.Impassable) != 0) { continue; }

            var playerInstance = Instantiate(playerPrefab);

            playerInstance.transform.position = tile.transform.position;
            playerInstance.MyGridPos = tile.coordinates;
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
        offsetX=Random.Range(0.0f, 99999.0f);
        offsetY=Random.Range(0.0f, 99999.0f);

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
        Vector3 position;
        position.x = x * (HexSettings.circumRadius * 1.5f);//(x + y * 0.5f - y / 2) * (HexSettings.inRadius * 2.0f);
        position.y = (y + x * 0.5f - x / 2) * (HexSettings.inRadius * 2.0f);//y *(HexSettings.circumRadius * 1.5f);
        position.z = 0f;

        HexTile tile = Instantiate(tilePrefab);
        tile.transform.SetParent(transform, false);
        tile.transform.localPosition = position;
        tile.coordinates = HexCoordinates.FromOffsetCoordinates(x, y);

        Text label = Instantiate(tileLabel);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.y);
        label.text = tile.coordinates.ToStringOnSeparateLines();

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
