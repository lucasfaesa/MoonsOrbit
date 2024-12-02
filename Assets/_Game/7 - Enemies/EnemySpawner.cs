using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enemy;
using Fusion;
using Networking;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : NetworkBehaviour
{
    [Header("SO's")]
    [SerializeField] private NetworkRunnerCallbacksSO networkRunnerCallbacks;
    [SerializeField] private NetworkPlayerCallbacksSO networkPlayerCallbacks;
    [Header("Settings")] 
    [SerializeField] private float enemyRespawnDelay = 10f;
    [Header("Enemies")] 
    [SerializeField] private HealthStatsSO enemyHealthStats;
    [SerializeField] private List<EnemyBehavior> enemies = new();
    [Header("Health Pack")]
    [SerializeField] private GameObject healthPack;
    
    
    private List<EnemyData> _enemiesDatas = new();
    
    [Networked] private NetworkBool EnemiesSetToPlayer { get; set; }
    
    private int? _ownerOfEnemiesId;

    public override void Spawned()
    {
        base.Spawned();
        SetEnemiesToPlayer(Runner);

        networkRunnerCallbacks.PlayerJoined += OnPlayerJoined;
        networkRunnerCallbacks.PlayerLeft += OnPlayerLeft;

        enemyHealthStats.Death += OnEnemyDeath;
        
        foreach (var enemyBehavior in enemies)
        {
            _enemiesDatas.Add(new EnemyData(enemyBehavior, 
                                enemyBehavior.GetComponent<NetworkObject>().Id.Raw, 
                                    enemyBehavior.transform.position));
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);

        enemyHealthStats.Death -= OnEnemyDeath;
        
        networkRunnerCallbacks.PlayerJoined -= OnPlayerJoined;
        networkRunnerCallbacks.PlayerLeft -= OnPlayerLeft;
    }

    private void OnPlayerJoined(NetworkRunner networkRunner, PlayerRef playerRef)
    {
        if(_ownerOfEnemiesId == null)
            _ownerOfEnemiesId = playerRef.PlayerId;
    }
    
    private void SetEnemiesToPlayer(NetworkRunner networkRunner)
    {
        if (EnemiesSetToPlayer) return;

        if (networkPlayerCallbacks.PlayersInGame.Count > 0 && _ownerOfEnemiesId == null)
            _ownerOfEnemiesId = networkPlayerCallbacks.PlayersInGame[0].PlayerId;
        
        
        Debug.Log($"Owner of the enemies is: {_ownerOfEnemiesId}");
        
        EnemiesSetToPlayer = true;
    }

    private void OnPlayerLeft(NetworkRunner networkRunner, PlayerRef playerRef)
    {
        //Debug.LogError("Player Left");
        _ownerOfEnemiesId = networkPlayerCallbacks.PlayersInGame.First(x=>x.PlayerId != playerRef.PlayerId).PlayerId;
        
        if (_ownerOfEnemiesId == networkRunner.LocalPlayer.PlayerId)
        {
            //Debug.LogError("You are the new owner of the enemies");

            var spawnedEnemies = FindObjectsByType<EnemyBehavior>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            foreach (var enemy in spawnedEnemies)
            {
                //Debug.LogError("Asked for state authority");
                enemy.IsPreparingToChangeStateAuthority();
                enemy.GetComponent<NetworkObject>().RequestStateAuthority();
            }
        }
    }

    private void OnEnemyDeath(uint networkId)
    {
        var enemyData = _enemiesDatas.Find(x=>x.Id == networkId);
        
        DespawnAndRespawnEnemy(enemyData);
        
    }
    
    private async void DespawnAndRespawnEnemy(EnemyData data)
    {
        await Task.Delay(TimeSpan.FromSeconds(2f));
        
        data.Behavior.Collider.enabled = false;
        data.Behavior.UnsubscribeFromEvents();
        data.Behavior.ToggleVisualsRPC(false);
        
        Runner.Spawn(healthPack, new Vector3(data.Behavior.transform.position.x, 
                                                data.Behavior.transform.position.y + healthPack.transform.position.y, 
                                                    data.Behavior.transform.position.z));
        
        await Task.Delay(TimeSpan.FromSeconds(enemyRespawnDelay));

        data.Behavior.Collider.enabled = true;
        data.Behavior.GetComponent<EnemyDamageable>().ResetHealth();
        data.Behavior.Spawned();
        data.Behavior.ToggleVisualsRPC(true);
    }

    private struct EnemyData
    {
        public EnemyBehavior Behavior;
        public uint Id;
        public Vector3 InitialPosition;

        public EnemyData(EnemyBehavior behavior, uint id, Vector3 initialPosition)
        {
            Behavior = behavior;
            Id = id;
            InitialPosition = initialPosition;
        }
    }
}
