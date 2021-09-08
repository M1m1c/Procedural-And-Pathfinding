using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MovableEntity
{

    public void OnPlayerWalking()
    {
    }
    public void OnPlayerStopping()
    {

    }

    public void OnPlayerRequestingPath()
    {
        if (oldPath.Count == 0) { return; }

    }
    private void GetRandomMoveGoal()
    {
        HexGrid.GetRandomWalkableTileWithin(this.transform.position, 5);
    }
}
