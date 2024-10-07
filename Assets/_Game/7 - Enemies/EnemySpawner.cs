using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Fusion;
using Networking;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("SO's")]
    [SerializeField] private NetworkRunnerCallbacksSO networkRunnerCallbacks;
    [Header("Enemy")]
    [SerializeField] private EnemyBehavior robotEnemy;
    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints = new();
    
    public void OnEnable()
    {
        networkRunnerCallbacks.ConnectedToServer += SpawnEnemies;
    }

    private void OnDisable()
    {
        networkRunnerCallbacks.ConnectedToServer -= SpawnEnemies;
    }

    private void SpawnEnemies(NetworkRunner networkRunner)
    {
        foreach (var spawnPoint in spawnPoints)
        {
            networkRunner.Spawn(robotEnemy, spawnPoint.position, spawnPoint.rotation, PlayerRef.None);
        }
    }
}
