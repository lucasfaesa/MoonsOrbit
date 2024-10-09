using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Fusion;
using Networking;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [Header("SO's")]
    [SerializeField] private NetworkRunnerCallbacksSO networkRunnerCallbacks;
    [Header("Enemy")]
    [SerializeField] private EnemyBehavior robotEnemy;
    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints = new();
    
    [Networked] public NetworkBool EnemiesSpawned { get; set; }


    public override void Spawned()
    {
        base.Spawned();
        SpawnEnemies(Runner);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        
    }


    private void SpawnEnemies(NetworkRunner networkRunner)
    {
        Debug.LogError($"Enemies spawned: {EnemiesSpawned}");
        
        if (EnemiesSpawned) return;
        
        EnemiesSpawned = true;
        foreach (var spawnPoint in spawnPoints)
        {
            networkRunner.Spawn(robotEnemy, spawnPoint.position, spawnPoint.rotation, PlayerRef.None);
        }
    }
}
