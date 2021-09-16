using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour, IHeapItem<HexTile>
{
    //public HexCoordinates coordinates;

    public Vector2Int Coordinates { get; private set; }

    [EnumFlags]
    public TileTags tileProperties;

    public Color highlightColor;

    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    public int HeapIndex { get; set; }

    public HexTile parent;

    public List<GameObject> Occupants { get; private set; }

    private SpriteRenderer spriteRenderer;

    private Color defaultColor;
    

    private int maxHealth = 3;
    private int health = 3;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Occupants = new List<GameObject>();
        defaultColor = spriteRenderer.color;
    }

    public void Setup(Vector2Int startCoord)
    {
        Coordinates = startCoord;
    }

    public int CompareTo(HexTile hexTileToCompare)
    {
        var compare = fCost.CompareTo(hexTileToCompare.fCost);

        if (compare == 0) { compare = hCost.CompareTo(hexTileToCompare.hCost); }

        return -compare;
    }

    //TODO add special circumstance where player can walk onto tiles with pickups
    public bool OccupyTile(GameObject potentialOccupier)
    {
        var retval = false;
        if (potentialOccupier)
        {
            if (Occupants.Count == 0)
            {
                Occupants.Add(potentialOccupier);
                retval = true;
            }
            else if (!Occupants.Contains(potentialOccupier))
            {
                Occupants.Add(potentialOccupier);
                retval = true;
            }
        }

        return retval;
    }

    public void DeOccupyTile(GameObject deOccupier)
    {
        if (deOccupier)
        {
            if (Occupants.Count != 0)
            {
                if (Occupants.Contains(deOccupier))
                {
                    Occupants.Remove(deOccupier);                
                }
            }
        }
    }

    public void HighLightTile(bool shouldLighten)
    {
        if (shouldLighten) { ChangeTileColor(highlightColor); }
        else { ChangeTileColor(defaultColor); }
    }

    public void ChangeTileColor(Color newColor)
    {
        spriteRenderer.color = newColor;
    }

    public void ReduceHealth()
    {
        health = Mathf.Clamp(health - 1, 0, maxHealth);
        if (health != 0) { return; }
        tileProperties = 0;
        defaultColor = Color.white;
        ChangeTileColor(Color.white);
    }
}
