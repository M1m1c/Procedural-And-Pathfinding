using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMaster : MonoBehaviour
{
    public EnemyController EnemyPrefab;

    private static EnemyMaster enemyMaster;

    private List<EnemyController> enemiesList = new List<EnemyController>();

    public static bool IsMoveGoalShared(HexTile myGoal,EnemyController requester)
    {
        var retval = false;
        if (myGoal)
        {
            foreach (var enemy in enemyMaster.enemiesList)
            {
                if (enemy == requester) { continue; }

                var otherGoal = enemy.GetGoalTile();
                if (!otherGoal) { continue; }

                if (otherGoal == myGoal)
                {
                    retval = true;
                    break;
                }
            }
        }    
        return retval;
    }

    public void SpawnEnemies(ref PlayerController playerInstance, int enemyCount)
    {
        playerInstance.OtherEntetiesCount = enemyCount;
        while (enemyCount > 0)
        {
            var tile = HexGrid.GetRandomEnemySpawn();
            if (!tile) { continue; }

            var enemyInstance = Instantiate(EnemyPrefab);
            enemiesList.Add(enemyInstance);
            enemyInstance.transform.position = tile.transform.position;
            enemyInstance.Setup(tile.Coordinates,tile);
            playerInstance.StartWalking.AddListener(enemyInstance.OnPlayerStartWalking);
            playerInstance.StoppingMovement.AddListener(enemyInstance.OnPlayerStopping);
            playerInstance.SelectionAction.AddListener(enemyInstance.OnPlayerSelectionAction);
            playerInstance.ContinuousWalking.AddListener(enemyInstance.OnPlayerStillMoving);
            enemyCount--;
        }
    }

    private void Awake()
    {
        enemyMaster = this;
    }

}
