using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public HexCoordinates coordinates;

    [EnumFlags]
    public TileTags tileProperties;

    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    public HexTile parent;

    public GameObject Occupant { get; private set; }

    private SpriteRenderer spriteRenderer;

    //TODO add special circumstance where player can walk onto tiles with pickups
    public bool OccupyTile(GameObject potentialOccupier)
    {
        var retval = false;
        if (potentialOccupier)
        {
            if (!Occupant)
            {
                Occupant = potentialOccupier;
                retval = true;
            }
        }

        return retval;   
    }

    public void ChangeTileColor(Color newColor)
    {
        spriteRenderer.color = newColor;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
