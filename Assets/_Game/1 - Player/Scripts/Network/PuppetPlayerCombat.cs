using DG.Tweening;
using Fusion;
using Networking;
using UnityEngine;
using UnityEngine.Pool;

public class PuppetPlayerCombat : NetworkBehaviour
{
    [Header("Components")] 
    [SerializeField] private Transform gunMuzzleRef;
    [SerializeField] private Transform pistolHolderTransform;
    [SerializeField] private WeaponStatsSO pistolStats;
    [SerializeField] private GunStatusChannelSO gunStatusChannel;
    [Header("Rig Aim Refs")]
    [SerializeField] private Transform pitchTransform;
    [SerializeField] private Transform rigTargetRef;
    [Space]
    [SerializeField] private BulletTrailBehavior bulletTrailPrefab;
    [SerializeField] private ParticleSystem muzzleFlashParticle;
    
    private IObjectPool<BulletTrailBehavior> _bulletTrailPool;
    
    private BulletTrailNetworkData _networkData;

    private bool _isLocalPlayer;
    private Vector3 _muzzleWorldVelocity;
    private Vector3 _lastMuzzlePosition;
    
    
    public override void Spawned()
    {
        base.Spawned();
        _isLocalPlayer = Object.HasStateAuthority;

        if (HasInputAuthority)
        {
            gunStatusChannel.StartedReloading += ExecuteReloadAnimationRPC;
        }
        
        
        _bulletTrailPool = new ObjectPool<BulletTrailBehavior>(CreateTrailPrefab, OnGetFromTrailPool,
            OnReleaseToTrailPool, OnDestroyTrailOnPool, false, 20, 100);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        
        if (!GetInput<PuppetPlayerInputData>(out var inputData))
            return;
        
        SetRigTargetReference(inputData.PlayerTransformNetworkData.PlayerCameraPos, inputData.PlayerTransformNetworkData.PlayerCameraForward);
        
        CalculateMuzzleVelocity();
        
        if (inputData.HasShotThisFrame)
        {
            InstantiateBulletRPC(inputData.BulletTrailNetworkData);
        }
    }

    private void SetRigTargetReference(Vector3 playerCameraWorldPos, Vector3 playerCameraForward)
    {
        
        Vector3 targetPosition;
                                                                                                                //ignoring only the player layer
        if (Physics.Raycast(playerCameraWorldPos, playerCameraForward, out var hit, 100f, ~LayerMask.NameToLayer("Player")))
        {
            targetPosition = hit.point;
        }
        else
        {
            targetPosition = pitchTransform.position + pitchTransform.transform.forward * 10f;
        }
        
        rigTargetRef.position = Vector3.Lerp(rigTargetRef.position, targetPosition, 20f * Runner.DeltaTime);
    }
    
    private void CalculateMuzzleVelocity()
    {
        _muzzleWorldVelocity = (gunMuzzleRef.position - _lastMuzzlePosition) / Runner.DeltaTime;
        _lastMuzzlePosition = gunMuzzleRef.position;
    }
    
    //this will be called by only the input authority and be executed on All (including the input authority), but the InvokeLocal removes that execution on input authority
    //because I want to invoke my bullets trails locally somewhere (in CombatFightState)
    [Rpc(RpcSources.InputAuthority, RpcTargets.All, InvokeLocal = false)]
    private void InstantiateBulletRPC(BulletTrailNetworkData data)
    {
        muzzleFlashParticle.Play();
        _networkData = data;
        _bulletTrailPool.Get();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All, InvokeLocal = false)]
    private void ExecuteReloadAnimationRPC()
    {
        pistolHolderTransform.DOLocalRotate(Vector3.forward * 1080f, pistolStats.ReloadTime, RotateMode.FastBeyond360).SetEase(Ease.OutBack, 1f);
    }
    
    private BulletTrailBehavior CreateTrailPrefab()
    {
        var newPoolObject = Instantiate(bulletTrailPrefab, gunMuzzleRef.position, Quaternion.LookRotation(_networkData.Direction));
        newPoolObject.ObjectPool = _bulletTrailPool;
        return newPoolObject;
    }

    private void OnReleaseToTrailPool(BulletTrailBehavior pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
    }
    
    private void OnGetFromTrailPool(BulletTrailBehavior pooledObject)
    {
        pooledObject.transform.SetPositionAndRotation(gunMuzzleRef.position, Quaternion.LookRotation(_networkData.Direction));
        pooledObject.transform.position += _muzzleWorldVelocity * Runner.DeltaTime; //moving the trail to fit neatly on the muzzle position, without this, moving and shooting the bullet starts to instantiate on the air instead of the muzzle pos 
        pooledObject.Initialize(_networkData.Target, _networkData.HitSomething, _networkData.TargetType, _networkData.TargetNormal);
        
        pooledObject.gameObject.SetActive(true);
    }
    
    private void OnDestroyTrailOnPool(BulletTrailBehavior pooledObject)
    {
        Destroy(pooledObject.gameObject);
    }
}
