using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public HexTile tilePrefab;

    public Text tileLabel;

    private Canvas gridCanvas;

    private int height = 8;
    private int width = 16;

    HexTile[] cells;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        cells = new HexTile[height * width];

        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateTile(x, y, i++);
            }
        }
    }

    void CreateTile(int x, int y, int i)
    {
        Vector3 position;
        position.x = x * (HexSettings.circumRadius * 1.5f);//(x + y * 0.5f - y / 2) * (HexSettings.inRadius * 2.0f);
        position.y = (y + x * 0.5f - x / 2) * (HexSettings.inRadius * 2.0f);//y *(HexSettings.circumRadius * 1.5f);
        position.z = 0f;

        HexTile cell = cells[i] = Instantiate(tilePrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;

        Text label = Instantiate(tileLabel);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.y);
        label.text = x.ToString() + "\n" + y.ToString();
    }
}
