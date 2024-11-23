using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Networking
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Header("SOs")] 
        [SerializeField] private NetworkRunnerCallbacksSO networkRunnerCallbacks;
        [SerializeField] private NetworkPlayerCallbacksSO networkPlayerCallbacks;
        [SerializeField] private HealthStatsSO playerHealthStats;
        [Header("Settings")] 
        [SerializeField] private float respawnDelay = 3f;
        
        [Header("Other")] 
        [SerializeField] private GameObject localPlayerPrefab;
        [SerializeField] private NetworkPrefabRef puppetPlayerPrefab;
        [Space] 
        [SerializeField] private List<Transform> spawnPointsTransform = new();

        private List<Transform> _puppetPlayersInGame = new();

        private GameObject _localPlayer;
        private CharacterController _localPlayerCharacterController; 
        
        private bool _spawned;
        
        private void OnEnable()
        {
            networkRunnerCallbacks.PlayerJoined += OnPlayerJoined;
            networkRunnerCallbacks.PlayerLeft += OnPlayerLeft;
            playerHealthStats.Death += RespawnDeadPlayer;
        }

        private void OnDisable()
        {
            networkRunnerCallbacks.PlayerJoined -= OnPlayerJoined;
            networkRunnerCallbacks.PlayerLeft -= OnPlayerLeft;
            playerHealthStats.Death -= RespawnDeadPlayer;
        }

        private void OnPlayerJoined(NetworkRunner networkRunner, PlayerRef playerRef)
        {
            if (playerRef != networkRunner.LocalPlayer)
                return;

            Transform spawnPoint = GetSpawnPoint();
            _localPlayer = Instantiate(localPlayerPrefab, spawnPoint.position, Quaternion.identity);
            _localPlayerCharacterController = _localPlayer.GetComponent<CharacterController>();
            _localPlayer.transform.SetParent(null);

            networkRunner.Spawn(puppetPlayerPrefab, spawnPoint.position, Quaternion.identity, playerRef);

            RefreshPuppetPlayerList();
            networkPlayerCallbacks.OnPlayerSpawn(networkRunner, playerRef);
        }
        
        private Transform GetSpawnPoint()
        {
            if (networkPlayerCallbacks.PlayersInGame.Count is 0 or 1)
            {
                int randIndex = Random.Range(0, spawnPointsTransform.Count);
                return spawnPointsTransform[randIndex];
            }
            else
            {
                var furthestSpawnPoints = GetFurthestSpawnPointsFromOtherPlayers();
                int randIndex = Random.Range(0, furthestSpawnPoints.Count);
                return furthestSpawnPoints[randIndex];
            }
        }
        
        private async void RefreshPuppetPlayerList()
        {
            await Task.Delay(TimeSpan.FromSeconds(1f));
            
            _puppetPlayersInGame.Clear();
            
            var puppetPlayersInGame = GameObject.FindGameObjectsWithTag("PuppetPlayer");
            
            Debug.LogError($"Quantity {puppetPlayersInGame.Length} Playerszz");
            
            foreach (var puppetPlayer in puppetPlayersInGame)
            {
                _puppetPlayersInGame.Add(puppetPlayer.transform);
            }
            
            Debug.LogError($"Quantity {_puppetPlayersInGame.Count} Players");
        }
        
        public List<Transform> GetFurthestSpawnPointsFromOtherPlayers()
        {
            // Dictionary to store spawn points and their total distances to all players
            Dictionary<Transform, float> spawnPointDistances = new Dictionary<Transform, float>();

            foreach (var spawnPoint in spawnPointsTransform)
            {
                float totalDistance = 0f;

                foreach (var player in _puppetPlayersInGame)
                {
                    totalDistance += Vector3.Distance(spawnPoint.position, player.position);
                }

                spawnPointDistances[spawnPoint] = totalDistance;
            }

            // Order the dictionary by distance in descending order and select the top 2 spawn points
            return spawnPointDistances.OrderByDescending(sp => sp.Value).Take(2).Select(sp => sp.Key).ToList();
        }

        private async void RespawnDeadPlayer(uint _)
        {
            _localPlayerCharacterController.enabled = false;
            
            await Task.Delay(TimeSpan.FromSeconds(respawnDelay));
            
            Transform spawnPoint = GetSpawnPoint();
            
            _localPlayer.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            
            playerHealthStats.OnRespawn();
            
            _localPlayerCharacterController.enabled = true;
        }
        
        private void OnPlayerLeft(NetworkRunner networkRunner, PlayerRef playerRef)
        {
            Debug.LogError("PlayerLeft");
            
            RefreshPuppetPlayerList();
            networkPlayerCallbacks.OnPlayerLeft(playerRef);
        }
    }
}

