using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using Fusion;
using LocalPlayer;
using Networking;
using UnityEngine;

public class PlayerCombatBehavior : SimulationBehaviour
{
    [Header("SOs")]
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] private PlayerStatsSO playerStats;

    [Space] 
    [SerializeField] private WeaponStatsSO pistolStats;
    [Header("Other")] 
    [SerializeField] private Transform bulletRef;
    [SerializeField] private BulletBehavior bulletPrefab;
    
    private StateMachine<PlayerCombatBehavior> _stateMachine = new();
    
    public InputReaderSO InputReader => inputReader;
    public PlayerStatsSO PlayerStats => playerStats;
    public Transform BulletRef => bulletRef;
    public BulletBehavior BulletPrefab => bulletPrefab;
    public WeaponStatsSO PistolStats => pistolStats;
    
    //----- State Machine things -----
    public CombatIdleState CombatIdleState { get; private set; }
    public CombatFightState CombatFightState { get; private set; }
    //---------
    
    
    public IEnumerator Start()
    {
        Debug.Log("Init");
        while (!NetworkManager.Instance.RunnerInitialized)
            yield return null;
        
        var runner = NetworkManager.Instance.NetworkRunner;
        Debug.Log($"Found Runner: {runner != null}");
        
        while (!runner.IsRunning)
            yield return null;
            
        
        runner.AddGlobal(this);
        Debug.Log("Added Global");
        
        
        inputReader.EnableInputActions();
        
        CombatIdleState = new CombatIdleState(this, _stateMachine);
        CombatFightState = new CombatFightState(this, _stateMachine);
        
        _stateMachine.Initialize(CombatIdleState);
    }

    public override void Render()
    {
        base.Render();
        _stateMachine.Update();
    }

}
