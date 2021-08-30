using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public HexCoordinates coordinates;

    [EnumFlags]
    public TileTags tileProperties;

    public GameObject Occupier { get; private set; }

    //TODO add special circumstance where player can walk onto tiles with pickups
    public bool OccupyTile(GameObject potentialOccupier)
    {
        var retval = false;
        if (potentialOccupier)
        {
            if (!Occupier)
            {
                Occupier = potentialOccupier;
            }
        }

        return retval;   
    }
}
