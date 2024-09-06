using Fusion;
using Networking;
using UnityEngine;

public class PuppetPlayerCombat : NetworkBehaviour
{
    [Header("Components")] 
    [SerializeField] private Transform gunMuzzleRef;
    [SerializeField] private Transform gunTransform;
    [Space]
    [SerializeField] private BulletTrailBehavior bulletTrail;
    [SerializeField] private ParticleSystem impactParticle;

    private bool _isLocalPlayer;

    public Vector3 _muzzleWorldVelocity;
    private Vector3 _lastMuzzlePosition;
    
    public override void Spawned()
    {
        base.Spawned();
        _isLocalPlayer = Object.HasStateAuthority;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (!GetInput<PuppetPlayerInputData>(out var inputData))
            return;
        
        var gunTransformNetData = inputData.GunTransformNetworkData;
        
        gunTransform.SetPositionAndRotation(gunTransformNetData.GunModelVisualPos, gunTransformNetData.GunModelVisualRot);
        gunMuzzleRef.SetPositionAndRotation(gunTransformNetData.GunMuzzleRefPos, gunTransformNetData.GunMuzzleRefRot);
        
        if (inputData.HasShotThisFrame)
            InstantiateBulletRPC(inputData.BulletTrailNetworkData);
    }

    private void CalculateMuzzleVelocity()
    {
        _muzzleWorldVelocity = (gunMuzzleRef.position - _lastMuzzlePosition) / Runner.DeltaTime;
        _lastMuzzlePosition = gunMuzzleRef.position;
    }
    
    //this will be called by only the input authority and be executed on All (including the input authority), but the InvokeLocal removes that execution on input authority
    //because I want to invoke my bullets trails locally somewhere
    [Rpc(RpcSources.InputAuthority, RpcTargets.All, InvokeLocal = false)]
    private void InstantiateBulletRPC(BulletTrailNetworkData data)
    {
        var newProjectile = Instantiate(bulletTrail, gunMuzzleRef.position, Quaternion.LookRotation(data.Direction));
        newProjectile.transform.position += _muzzleWorldVelocity * Runner.DeltaTime;
        
        newProjectile.Initialize(data.Target, data.TrailSpeed, data.HitSomething, data.TargetType, data.TargetNormal);
    }
}
