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

    public GameObject occupant { get; private set; }

    //TODO add special circumstance where player can walk onto tiles with pickups
    public bool OccupyTile(GameObject potentialOccupier)
    {
        var retval = false;
        if (potentialOccupier)
        {
            if (!occupant)
            {
                occupant = potentialOccupier;
                retval = true;
            }
        }

        return retval;   
    }
}
