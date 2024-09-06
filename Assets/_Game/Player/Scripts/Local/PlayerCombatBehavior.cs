using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using Fusion;
using LocalPlayer;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatBehavior : MonoBehaviour
{
    [Header("SOs")]
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] private PlayerStatsSO playerStats;
    [Space] 
    [SerializeField] private ParticleSystem muzzleFlashParticle;
    [SerializeField] private WeaponStatsSO pistolStats;
    [Header("Networking")] 
    [SerializeField] private LocalPlayerToPuppetSynchronizer localPlayerToPuppetSynchronizer;
    [Header("Other")]
    [SerializeField] private Transform gunMuzzleRef;
    
    private StateMachine<PlayerCombatBehavior> _stateMachine = new();
    
    public InputReaderSO InputReader => inputReader;
    public PlayerStatsSO PlayerStats => playerStats;
    public Transform GunMuzzleRef => gunMuzzleRef;
    public WeaponStatsSO PistolStats => pistolStats;
    public LocalPlayerToPuppetSynchronizer LocalPlayerToPuppetSynchronizer => localPlayerToPuppetSynchronizer;
    public ParticleSystem MuzzleFlashParticle => muzzleFlashParticle;
    
    public Vector3 MuzzleWorldVelocity { get; set; }
    
    //----- State Machine things -----
    public CombatIdleState CombatIdleState { get; private set; }
    public CombatFightState CombatFightState { get; private set; }
    //---------
    
    private Vector3 _lastMuzzlePosition;
    
    public void Start()
    {
        inputReader.EnableInputActions();
        
        CombatIdleState = new CombatIdleState(this, _stateMachine);
        CombatFightState = new CombatFightState(this, _stateMachine);
        
        _stateMachine.Initialize(CombatIdleState);
    }

    private void Update()
    {
        MuzzleWorldVelocity = (GunMuzzleRef.position - _lastMuzzlePosition) / Time.deltaTime;
        _lastMuzzlePosition = GunMuzzleRef.position;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            Time.timeScale = 0.05f;
        if (Keyboard.current.digit1Key.wasReleasedThisFrame)
            Time.timeScale = 1f;
        
        _stateMachine.Update();
    }
    
}
