using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using DG.Tweening;
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
    [SerializeField] private GunStatusChannelSO _gunStatusChannel;
    [Space] [Header("Refs")] 
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera weaponCamera;
    [SerializeField] private Transform playerWeaponHolder;
    [SerializeField] private ParticleSystem muzzleFlashParticle;
    [SerializeField] private BulletTrailBehavior bulletTrailPrefab;
    [SerializeField] private Transform gunMuzzleRef;
    [Header("Stats")]
    [SerializeField] private WeaponStatsSO pistolStats;
    [Header("Networking")] 
    [SerializeField] private LocalPlayerToPuppetSynchronizer localPlayerToPuppetSynchronizer;
    
    private StateMachine<PlayerCombatBehavior> _stateMachine = new();
    private int _bulletsLeft;
    private Vector3 _lastMuzzlePosition;
    private (Vector3 pos, Vector3 rot) _weaponHolderDefaultPositionAndRotation;
    private (Vector3 pos, Vector3 rot) _weaponHolderAimingPositionAndRotation = (new Vector3(0f, -0.151f, 0.371f), Vector3.zero);
    private float _defaultFov;
    private float _aimFov = 35f;
    private bool _isAiming;
    private float _aimAnimationDuration = 0.1f;
    
    private Sequence _aimSequence;
    private Sequence _leaveAimSequence;
    
    public InputReaderSO InputReader => inputReader;
    public Transform GunMuzzleRef => gunMuzzleRef;
    public WeaponStatsSO PistolStats => pistolStats;
    public LocalPlayerToPuppetSynchronizer LocalPlayerToPuppetSynchronizer => localPlayerToPuppetSynchronizer;
    public ParticleSystem MuzzleFlashParticle => muzzleFlashParticle;

    public Transform PlayerWeaponHolder => playerWeaponHolder;
    public BulletTrailBehavior BulletTrailPrefab => bulletTrailPrefab;
    public Vector3 MuzzleWorldVelocity { get; private set; }
    public Transform PlayerCameraTransform => playerCamera.transform;
    public bool IsAiming => _isAiming;
    public bool CanAim { get; set; } = true;

    //----- State Machine things -----
    public CombatIdleState CombatIdleState { get; private set; }
    public CombatFightState CombatFightState { get; private set; }
    public CombatReloadState CombatReloadState { get; private set; }
    //---------
    
    
    public int BulletsLeft
    {
        get => _bulletsLeft;
        set
        {
            _bulletsLeft = value;
            _gunStatusChannel.CurrentBulletsAmount = _bulletsLeft;
        }
    }

    public void Start()
    {
        _defaultFov = playerCamera.fieldOfView;
        _weaponHolderDefaultPositionAndRotation = (playerWeaponHolder.localPosition, playerWeaponHolder.localRotation.eulerAngles);
        
        _aimSequence = DOTween.Sequence().Append(playerCamera.DOFieldOfView(_aimFov, _aimAnimationDuration).SetEase(Ease.InOutSine))
            .Join(weaponCamera.DOFieldOfView(_aimFov, _aimAnimationDuration).SetEase(Ease.InOutSine))
            .Join(playerWeaponHolder.DOLocalMove(_weaponHolderAimingPositionAndRotation.pos, _aimAnimationDuration).SetEase(Ease.InOutSine))
            .Join(playerWeaponHolder.DOLocalRotate(_weaponHolderAimingPositionAndRotation.rot, _aimAnimationDuration).SetEase(Ease.InOutSine))
            .OnComplete(()=>
            {
                _isAiming = true;
            })
            .Pause().SetAutoKill(false);
        
        _leaveAimSequence = 
            DOTween.Sequence().OnStart(() =>
            {
                _isAiming = false;
            })
            .Append(playerCamera.DOFieldOfView(_defaultFov, _aimAnimationDuration).SetEase(Ease.InOutSine))
            .Join(weaponCamera.DOFieldOfView(_defaultFov, _aimAnimationDuration).SetEase(Ease.InOutSine))
            .Join(playerWeaponHolder.DOLocalMove(_weaponHolderDefaultPositionAndRotation.pos, _aimAnimationDuration).SetEase(Ease.InOutSine))
            .Join(playerWeaponHolder.DOLocalRotate(_weaponHolderDefaultPositionAndRotation.rot, _aimAnimationDuration).SetEase(Ease.InOutSine))
            .Pause().SetAutoKill(false);
        
        BulletsLeft = pistolStats.BulletsPerClip;
        
        inputReader.EnableInputActions();
        
        CombatIdleState = new CombatIdleState(this, _stateMachine);
        CombatFightState = new CombatFightState(this, _stateMachine);
        CombatReloadState = new CombatReloadState(this, _stateMachine);
            
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
        
        if(CanAim)
            ToggleAim(inputReader.AimStatus);
    }

    public void ToggleAim(bool aim)
    {
        if (aim == _isAiming) return;
        
        _gunStatusChannel.OnIsAiming(aim);
        
        if (aim)
            EnterAimMode();
        else
            LeaveAimMode();
    }

    private void EnterAimMode()
    {
        _isAiming = true;

        _leaveAimSequence.Pause();
        _aimSequence.Restart();
    }

    private void LeaveAimMode()
    {
        _isAiming = false;

        _aimSequence.Pause();
        _leaveAimSequence.Restart();
        
    }
}
