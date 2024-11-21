using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using Fusion;
using UnityEngine;
using UnityEngine.Pool;

namespace Enemy
{
    public class EnemyBehaviorAttack : State<EnemyBehavior>
    {
        
        public EnemyBehaviorAttack(EnemyBehavior context, StateMachine<EnemyBehavior> stateMachine) : base(context, stateMachine)
        {
        }

        private readonly int _shootingAnimatorParameter = Animator.StringToHash("Shooting");
        
        private Vector3 _randomSpread;
        private Vector3 _spreadDirection;
        
        
        public override void Enter()
        {
//            Debug.Log("<color=purple>Enemy Attack State</color>");
            
            base.Enter();
            context.Attacking = true;
            context.InCombat = true;
            context.ShootingAnimationEvent += ShootForward;
            context.Animator.SetBool(_shootingAnimatorParameter, true);
        }

        public override void LogicUpdate()
        {
            if (context.Target == null)
                return;
            
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
            context.Attacking = false;
            context.InCombat = false;
            context.ShootingAnimationEvent -= ShootForward;
            context.MuzzleFlashParticle.Stop();
            context.Animator.SetBool(_shootingAnimatorParameter, false);
        }

        private void ShootForward()
        {
            Vector3 forwardDirection = context.GunMuzzle.forward;
            
            _spreadDirection = (forwardDirection + _randomSpread).normalized;
            
            _randomSpread = new Vector3(
                Random.Range(-context.EnemyStats.BulletSpread.x, context.EnemyStats.BulletSpread.x),
                Random.Range(-context.EnemyStats.BulletSpread.y, context.EnemyStats.BulletSpread.y),
                Random.Range(-context.EnemyStats.BulletSpread.z, context.EnemyStats.BulletSpread.z)
            );
            
            context.MuzzleFlashParticle.Play();
            
            //_physicalBulletPool.Get();

            context.InstantiateBulletRPC(_spreadDirection);
        }
    }
}
