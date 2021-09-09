using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMaster : MonoBehaviour
{
    public EnemyController EnemyPrefab;

    private static EnemyMaster enemyMaster;

    private List<EnemyController> enemiesList = new List<EnemyController>();

    public void SpawnEnemies(ref PlayerController playerInstance, int enemyCount)
    {
        while (enemyCount > 0)
        {
            var tile = HexGrid.GetRandomEnemySpawn();
            if (!tile) { continue; }

            var enemyInstance = Instantiate(EnemyPrefab);
            enemiesList.Add(enemyInstance);
            enemyInstance.transform.position = tile.transform.position;
            enemyInstance.Setup(tile.Coordinates);
            playerInstance.StartWalking.AddListener(enemyInstance.OnPlayerStartWalking);
            playerInstance.StoppingMovement.AddListener(enemyInstance.OnPlayerStopping);
            playerInstance.RequestingPath.AddListener(enemyInstance.OnPlayerRequestingPath);
            playerInstance.ContinuousWalking.AddListener(enemyInstance.OnPlayerStillMoving);
            enemyCount--;
        }
    }

    private void Awake()
    {
        enemyMaster = this;
    }

}
