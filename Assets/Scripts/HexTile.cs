using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public HexCoordinates coordinates;

    [EnumFlags]
    public TileTags tileProperties;
}
