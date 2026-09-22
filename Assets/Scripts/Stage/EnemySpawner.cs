using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 결정")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private int minSpawnCount = 2;
    [SerializeField] private int maxSpawnCount = 4;

    [Header("래퍼런스")]
    [SerializeField] private Transform player;

    private int aliveCount;

    public event Action OnAllEnemiesDead;


    public void SpawnEnemies()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        int maxCount = Mathf.Min(maxSpawnCount, spawnPoints.Length);
        int minCount = Mathf.Min(minSpawnCount, maxCount);
        int spawnCount = Random.Range(minCount, maxCount + 1);
        aliveCount = 0;

        List<int> pointIndexes = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            pointIndexes.Add(i);
        }
        //SpawnPoint 순서를 랜덤하게 섞음
        for (int i = 0; i < pointIndexes.Count; i++)
        {
            int randomIndex = Random.Range(i, pointIndexes.Count);
            (pointIndexes[i], pointIndexes[randomIndex]) = (pointIndexes[randomIndex], pointIndexes[i]);
        }
        for (int i = 0; i < spawnCount;  i++)
        {
            Transform point = spawnPoints[pointIndexes[i]];
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemyObject = Instantiate(prefab, point.position, Quaternion.identity, transform);

            EnemyController controller = enemyObject.GetComponent<EnemyController>();
            if (controller != null) controller.SetPlayer(player);

            EnemyStatus status = enemyObject.GetComponent<EnemyStatus>();
            if (status != null)
            {
                status.OnDead += EnemyDead;
                aliveCount++;
            }
        }
        //혹시 정상 EnemyStatus가 하나도 없었을 경우
        if (aliveCount == 0) OnAllEnemiesDead?.Invoke();
    }
    private void EnemyDead(EnemyStatus enemy)
    {
        enemy.OnDead -= EnemyDead;
        aliveCount--;
        if (aliveCount > 0) return;
        aliveCount = 0;
        OnAllEnemiesDead?.Invoke();
    }

}
