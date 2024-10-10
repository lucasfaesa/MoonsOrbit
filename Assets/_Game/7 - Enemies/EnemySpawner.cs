using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using Fusion;
using Networking;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [Header("SO's")]
    [SerializeField] private NetworkRunnerCallbacksSO networkRunnerCallbacks;
    [SerializeField] private NetworkPlayerCallbacksSO networkPlayerCallbacks;
    [Header("Enemy")]
    [SerializeField] private EnemyBehavior robotEnemyPrefab;
    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints = new();
    
    [Networked] private NetworkBool EnemiesSpawned { get; set; }
    
    private int? _ownerOfEnemiesId;

    public override void Spawned()
    {
        base.Spawned();
        SpawnEnemies(Runner);

        networkRunnerCallbacks.PlayerJoined += OnPlayerJoined;
        networkRunnerCallbacks.PlayerLeft += OnPlayerLeft;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);

        networkRunnerCallbacks.PlayerJoined -= OnPlayerJoined;
        networkRunnerCallbacks.PlayerLeft -= OnPlayerLeft;
    }

    private void OnPlayerJoined(NetworkRunner networkRunner, PlayerRef playerRef)
    {
        if(_ownerOfEnemiesId == null)
            _ownerOfEnemiesId = playerRef.PlayerId;
    }
    
    private void SpawnEnemies(NetworkRunner networkRunner)
    {
        Debug.LogError($"Enemies spawned: {EnemiesSpawned}");
        
        if (EnemiesSpawned) return;

        if (networkPlayerCallbacks.PlayersInGame.Count > 0 && _ownerOfEnemiesId == null)
            _ownerOfEnemiesId = networkPlayerCallbacks.PlayersInGame[0].PlayerId;
        
        
        Debug.Log($"Owner of the enemies is: {_ownerOfEnemiesId}");
        
        EnemiesSpawned = true;
        foreach (var spawnPoint in spawnPoints)
        {
            var enemy = networkRunner.Spawn(robotEnemyPrefab, spawnPoint.position, spawnPoint.rotation, PlayerRef.None);
        }
    }

    private void OnPlayerLeft(NetworkRunner networkRunner, PlayerRef playerRef)
    {
        Debug.Log("Player Left");
        _ownerOfEnemiesId = networkPlayerCallbacks.PlayersInGame.First(x=>x.PlayerId != playerRef.PlayerId).PlayerId;
        
        if (_ownerOfEnemiesId == networkRunner.LocalPlayer.PlayerId)
        {
            Debug.Log("You are the new owner of the enemies");

            var spawnedEnemies = FindObjectsByType<EnemyBehavior>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            foreach (var enemy in spawnedEnemies)
            {
                Debug.Log("Asked for state authority");
                enemy.GetComponent<NetworkObject>().RequestStateAuthority();
            }
        }
    }
}
