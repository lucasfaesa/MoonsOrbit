using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;
using UnityEngine.Pool;

namespace Enemy
{
    public class EnemyBehaviorAttack : State<EnemyBehavior>
    {
        
        public EnemyBehaviorAttack(EnemyBehavior context, StateMachine<EnemyBehavior> stateMachine) : base(context, stateMachine)
        {
            InitPools();
        }

        private readonly int _shootingAnimatorParameter = Animator.StringToHash("Shooting");
        
        private IObjectPool<PhysicalBulletBehavior> _physicalBulletPool;

        private Vector3 _randomSpread;
        private Vector3 _spreadDirection;
        
        
        private void InitPools()
        {
            _physicalBulletPool = new ObjectPool<PhysicalBulletBehavior>(CreateTrailPrefab, OnGetFromTrailPool,
                OnReleaseToTrailPool, OnDestroyTrailOnPool, false, 20, 100);
        }
        
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
            
            _spreadDirection = (forwardDirection + _randomSpread).normalized;
            
            _randomSpread = new Vector3(
                Random.Range(-context.EnemyStats.BulletSpread.x, context.EnemyStats.BulletSpread.x),
                Random.Range(-context.EnemyStats.BulletSpread.y, context.EnemyStats.BulletSpread.y),
                Random.Range(-context.EnemyStats.BulletSpread.z, context.EnemyStats.BulletSpread.z)
            );
            
            context.MuzzleFlashParticle.Play();
            
            _physicalBulletPool.Get();
            
        }
        
        private PhysicalBulletBehavior CreateTrailPrefab()
        {
            var newPoolObject = Object.Instantiate(context.PhysicalBulletPrefab, context.GunMuzzle.position, Quaternion.LookRotation(_spreadDirection));
            newPoolObject.ObjectPool = _physicalBulletPool;
            return newPoolObject;
        }

        private void OnReleaseToTrailPool(PhysicalBulletBehavior pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
        }
    
        private void OnGetFromTrailPool(PhysicalBulletBehavior pooledObject)
        {
            pooledObject.transform.SetPositionAndRotation(context.GunMuzzle.position, Quaternion.LookRotation(_spreadDirection));
            
            var targetPoint = _spreadDirection * 1000f;
            
            pooledObject.Initialize(targetPoint, context.EnemyStats.BulletSpeed);
        
            pooledObject.gameObject.SetActive(true);
        }
    
        private void OnDestroyTrailOnPool(PhysicalBulletBehavior pooledObject)
        {
            Object.Destroy(pooledObject.gameObject);
        }
    }
}
