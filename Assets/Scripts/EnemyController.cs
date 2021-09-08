using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MovableEntity
{

    public void OnPlayerWalking()
    {
        Debug.Log("player is walking");
    }
    public void OnPlayerStopping()
    {

    }
    private void GetRandomMoveGoal()
    {
        HexGrid.GetRandomWalkableTileWithin(this.transform.position, 5);
    }
}
