using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

namespace Enemy
{
    public class EnemyBehaviorAttack : State<EnemyBehavior>
    {
        
        public EnemyBehaviorAttack(EnemyBehavior context, StateMachine<EnemyBehavior> stateMachine) : base(context, stateMachine)
        {
            
        }

        private readonly int _shootingAnimatorParameter = Animator.StringToHash("Shooting");
        
        public override void Enter()
        {
            Debug.Log("<color=purple>Enemy Attack State</color>");
            
            base.Enter();
            context.ShootingAnimationEvent += ShootForward;
            context.Animator.SetBool(_shootingAnimatorParameter, true);
        }

        public override void LogicUpdate()
        {
            if (Vector3.Distance(context.transform.position, context.Target.position) >=
                context.EnemyStats.AttackDistance)
            {
                stateMachine.ChangeState(context.BehaviorChaseState);
            }
            
            context.LookAtTarget();
        }

        public override void PhysicsUpdate()
        {
            
        }

        public override void Exit()
        {
            base.Exit();
            context.ShootingAnimationEvent -= ShootForward;
            context.MuzzleFlashParticle.Stop();
            context.Animator.SetBool(_shootingAnimatorParameter, false);
        }

        private void ShootForward()
        {
            Vector3 forwardDirection = context.GunMuzzle.forward;

            // Add bullet spread to the direction by adding random values to the direction vector
            Vector3 randomSpread = new Vector3(
                Random.Range(-context.EnemyStats.BulletSpread.x, context.EnemyStats.BulletSpread.x),
                Random.Range(-context.EnemyStats.BulletSpread.y, context.EnemyStats.BulletSpread.y),
                Random.Range(-context.EnemyStats.BulletSpread.z, context.EnemyStats.BulletSpread.z)
            );

            // Apply the random spread to the forward direction and normalize it to ensure it stays a direction vector
            Vector3 spreadDirection = (forwardDirection + randomSpread).normalized;

            // Instantiate the bullet with the new spread direction
            var bullet = Object.Instantiate(
                context.PhysicalBulletPrefab, 
                context.GunMuzzle.position, 
                Quaternion.LookRotation(spreadDirection)
            );

            // Calculate the target point, a faraway point in the spread direction
            var _targetPoint = spreadDirection * 1000f;

            // Initialize the bullet with the target point and bullet speed
            context.MuzzleFlashParticle.Play();
            bullet.Initialize(_targetPoint, context.EnemyStats.BulletSpeed);
        }
    }
}
