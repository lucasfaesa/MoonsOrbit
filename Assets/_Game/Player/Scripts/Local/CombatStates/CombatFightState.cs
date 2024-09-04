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
            
            //TODO instantiate the particle, not play LOL
            context.PistolStats.MuzzleFlashParticle.Play();

            var direction = GetDirection();
            RaycastHit hit;
            Vector3 targetPoint;

            // Check if the raycast hits something
            if (Physics.Raycast(context.BulletRef.position, direction, out hit, float.MaxValue, _weaponStats.Mask))
            {
                targetPoint = hit.point;  // If hit, the target point is the hit point
            }
            else
            {
                // If no hit, calculate a faraway point in the shooting direction
                targetPoint = context.BulletRef.position + direction * 1000f; // 1000 units away
            }

            // Instantiate the projectile
            ProjectileBase newProjectile = Object.Instantiate(_weaponStats.BulletTrail, context.BulletRef.position,
                Quaternion.LookRotation(direction));
        
            // Start the trail coroutine
            context.StartCoroutine(SpawnTrail(newProjectile, targetPoint, hit));
        }

        if (!context.InputReader.ShootStatus)
            stateMachine.ChangeState(context.CombatIdleState);
    }

    private IEnumerator SpawnTrail(ProjectileBase trail, Vector3 targetPoint, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        float distance = Vector3.Distance(startPosition, targetPoint);
        float duration = distance / _weaponStats.TrailSpeed; 

        while (time < duration)
        {
            trail.transform.position = Vector3.Lerp(startPosition, targetPoint, time / duration);
            time += Time.deltaTime;

            yield return null;
        }
    
        // Destroy the trail after reaching the target point
        Object.Destroy(trail.gameObject);

        // If there was a hit, instantiate impact particle
        if (hit.collider != null)
        {
            Object.Instantiate(_weaponStats.ImpactParticle, hit.point, Quaternion.LookRotation(hit.normal));
        }

        // Optional: Destroy the trail after some time
        //Object.Destroy(trail.gameObject, 5f);
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
