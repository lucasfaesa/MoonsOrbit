using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using Unity.FPS.Game;
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
        if (context.Runner.LocalRenderTime - _lastShotTime > _weaponStats.DelayBetweenShots)
        {
            _lastShotTime = context.Runner.LocalRenderTime;

            //var bullet = context.Runner.Spawn(context.BulletPrefab, context.BulletRef.position, context.BulletRef.rotation);
            //bullet.gameObject.SetActive(true);
            
            //TODO instantiate the particle, not play LOL
            context.PistolStats.MuzzleFlashParticle.Play();

            var direction = GetDirection();
            
            if (Physics.Raycast(context.BulletRef.position, direction, out RaycastHit hit,
                    float.MaxValue, _weaponStats.Mask))
            {
                ProjectileBase newProjectile = Object.Instantiate(_weaponStats.BulletTrail, context.BulletRef.position,
                    Quaternion.LookRotation(context.BulletRef.transform.forward));
                newProjectile.Shoot(context.MuzzleWorldVelocity);
                /*GameObject trail = Object.Instantiate(_weaponStats.BulletTrail, context.BulletRef.position,
                    Quaternion.identity);*/
                
                //context.StartCoroutine(SpawnTrail(trail, hit));
            }
        }

        if (!context.InputReader.ShootStatus)
            stateMachine.ChangeState(context.CombatIdleState);
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        float distance = Vector3.Distance(startPosition, hit.point);
        float duration = distance / _weaponStats.TrailSpeed; 

        while (time < duration)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time / duration);
            time += Time.deltaTime;
    
            yield return null;
        }

        trail.transform.position = hit.point;
        Object.Instantiate(_weaponStats.ImpactParticle, hit.point, Quaternion.LookRotation(hit.normal));

        // Optional: Destroy the trail after some time
        Object.Destroy(trail.gameObject, trail.time);
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = context.BulletRef.forward;

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
