using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public HexTile tilePrefab;

    public Text tileLabel;

    private Canvas gridCanvas;

    [SerializeField] private int height = 8;
    [SerializeField] private int width = 16;

    [SerializeField] private float scale = 10.0f;
    [SerializeField] private float offsetX = 10.0f;
    [SerializeField] private float offsetY = 10.0f;

    List<HexTile> tiles;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        tiles = new List<HexTile>();

        GenerateGrid();
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
                float clampedPerlin = Mathf.Clamp(rawPerlin, 0.0f, 1.0f);
                Debug.Log("" + clampedPerlin);

                if (clampedPerlin > 0.3f)
                {
                    tiles.Add(CreateTile(x, y));
                }


            }
        }
    }

    HexTile CreateTile(int x, int y)
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
}
