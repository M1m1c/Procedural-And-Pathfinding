using UnityEngine;

public static class HexSettings
{
    //This is the radius of the outer circle that crosses all corenrs,
    //determined by drawing a line form the center of the hex to any corner.
    //It is used to determine the size of each side of the virtual hex or the space inbetween the hexes.
    public const float circumRadius = 6f; 

    //This describes the radius of thee inner cricle located in a hexagon
    public const float inRadius = circumRadius * equationValue;

    //this is the result from the equation Mathf.sqrt(3)/2.
    //Best described here: https://www.omnicalculator.com/math/hexagon
    //And here  https://www.youtube.com/watch?v=A8YVK7kxZOw
    private const float equationValue = 0.866025404f;
}
