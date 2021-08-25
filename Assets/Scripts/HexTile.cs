using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public HexCoordinates coordinates;

    private Vector3 worldPos;

    public Vector3 worldPosition { get { return worldPos; } set { worldPos = value; } }

}
