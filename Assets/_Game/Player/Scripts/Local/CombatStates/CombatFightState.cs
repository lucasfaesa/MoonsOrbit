using System.Collections;
using DesignPatterns;
using Fusion;
using Networking;
using UnityEngine;

public class CombatFightState : State<PlayerCombatBehavior>
{
    public CombatFightState(PlayerCombatBehavior context, StateMachine<PlayerCombatBehavior> stateMachine) : base(context, stateMachine)
    {
    }
    
    private float _lastShotTime = 0f;
    private WeaponStatsSO _weaponStats;

    
    public override void Enter()
    {
        //Time.timeScale = 0.1f;
        Debug.Log("<color=magenta>Entered combat fight state</color>");
        _weaponStats = context.PistolStats;
    }

    public override void LogicUpdate()
    {
        if (Time.time - _lastShotTime > _weaponStats.DelayBetweenShots)
        {
            Shoot();
        }

        if (!context.InputReader.ShootStatus)
            stateMachine.ChangeState(context.CombatIdleState);
    }

    private void Shoot()
    {
        _lastShotTime = Time.time;
            
        //TODO instantiate the particle, not play LOL
        context.PistolStats.MuzzleFlashParticle.Play();

        var direction = GetDirection();
        RaycastHit hit;
        Vector3 targetPoint;
        bool hitSomething = false;
        
        if (Physics.Raycast(context.GunMuzzleRef.position, direction, out hit, float.MaxValue, _weaponStats.Mask))
        {
            targetPoint = hit.point;
            hitSomething = true;
        }
        else
            targetPoint = context.GunMuzzleRef.position + direction * 1000f; // If no hit, calculate a faraway point in the shooting direction
            
        
        var newProjectile = Object.Instantiate(_weaponStats.BulletTrail, context.GunMuzzleRef.position, Quaternion.LookRotation(direction));
        //moving the trail to fit neatly on the muzzle position, without this, moving and shooting the bullet starts to instantiate on the air instead of the muzzle pos
        newProjectile.transform.position += context.MuzzleWorldVelocity * Time.deltaTime; 
        newProjectile.Initialize(targetPoint, _weaponStats.TrailSpeed, hitSomething, _weaponStats.ImpactParticle, hit.normal);
        
        //sending trail stats to puppet
        context.LocalPlayerToPuppetSynchronizer.SetBulletTrailData(new BulletTrailNetworkData(targetPoint, hitSomething, hit.normal,direction, _weaponStats.TrailSpeed));
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = context.GunMuzzleRef.forward;

        if (_weaponStats.HasBulletSpread)
        {
            var spreadVariance = _weaponStats.BulletSpreadVariance;
            var spreadX = Random.Range(-spreadVariance.x, spreadVariance.x);
            var spreadY = Random.Range(-spreadVariance.y, spreadVariance.y);
            var spreadZ = Random.Range(-spreadVariance.z, spreadVariance.z);
            
            direction += new Vector3(spreadX,spreadY,spreadZ);
            
            direction.Normalize();
        }
        

        return direction;
    }

    public override void PhysicsUpdate()
    {

    }

    public override void Exit()
    {

    }
}
