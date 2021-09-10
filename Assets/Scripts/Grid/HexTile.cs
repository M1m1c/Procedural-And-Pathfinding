using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour, IHeapItem<HexTile>
{
    //public HexCoordinates coordinates;

    public Vector2Int Coordinates { get; private set; }

    [EnumFlags]
    public TileTags tileProperties;

    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    public int HeapIndex { get; set; }

    public HexTile parent;

    public List<GameObject> Occupants { get; private set; }

    private SpriteRenderer spriteRenderer;

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

    public bool DeOccupyTile(GameObject deOccupier)
    {
        var retval = false;
        if (deOccupier)
        {
            if (Occupants.Count != 0)
            {
                if (Occupants.Contains(deOccupier))
                {
                    Occupants.Remove(deOccupier);
                    retval = true;
                }
            }
        }

        return retval;
    }

    public void ChangeTileColor(Color newColor)
    {
        spriteRenderer.color = newColor;
    }

    public void Setup(Vector2Int startCoord)
    {
        Coordinates = startCoord;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Occupants = new List<GameObject>();
    }
}
