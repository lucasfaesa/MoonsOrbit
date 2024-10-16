using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using Fusion;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;


namespace Enemy
{
    public class EnemyBehavior : NetworkBehaviour, IStateAuthorityChanged
    {

        [Header("SOs")]
        [SerializeField] private EnemyStatsSO enemyStats;
        [Header("References")] 
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform gunMuzzle;
        [SerializeField] private ParticleSystem muzzleFlashParticle;
        [SerializeField] private PhysicalBulletBehavior physicalBulletPrefab;
        [Header("Patrol Points")] 
        [SerializeField] private Transform patrolLocations;
        [SerializeField] private List<Transform> patrolPoints = new();
        
        public Transform Target { get; set; }
        public EnemyStatsSO EnemyStats => enemyStats;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public Animator Animator => animator;
        public Transform GunMuzzle => gunMuzzle;
        public ParticleSystem MuzzleFlashParticle => muzzleFlashParticle;
        public PhysicalBulletBehavior PhysicalBulletPrefab => physicalBulletPrefab;
        public List<Transform> PatrolPoints => patrolPoints;
        
        public event Action ShootingAnimationEvent;
        
        //----- State Machine things -----
        public EnemyBehaviorIdle BehaviorIdleState { get; private set; }
        public EnemyBehaviorPatrol BehaviorPatrolState { get; private set; }
        public EnemyBehaviorChase BehaviorChaseState { get; private set; }
        public EnemyBehaviorAttack BehaviorAttackState { get; private set; }
        
        private StateMachine<EnemyBehavior> _stateMachine = new();
        //---------
        
        private readonly int _speedAnimatorParameter = Animator.StringToHash("Speed");
        private float _updatePathTimer = 0;
        private Collider[] _hitColliders = new Collider[10];

        public void StateAuthorityChanged()
        {
            if (Object.HasStateAuthority)
            {
                Debug.Log("I have state authority");
                
                Spawned();
            }
        }
        
        public override void Spawned()
        {
            base.Spawned();

            if (!HasStateAuthority)
            {
                navMeshAgent.enabled = false;
                return;
            }
            
            navMeshAgent.enabled = true;
            patrolLocations.SetParent(null);
            
            
            BehaviorIdleState = new EnemyBehaviorIdle(this, _stateMachine);
            BehaviorPatrolState = new EnemyBehaviorPatrol(this, _stateMachine);
            BehaviorChaseState = new EnemyBehaviorChase(this, _stateMachine);
            BehaviorAttackState = new EnemyBehaviorAttack(this, _stateMachine);
            
            _stateMachine.Initialize(BehaviorIdleState);
        }


        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            
            if (!HasStateAuthority)
                return;
            
            _stateMachine.Update();
        }
        
        public void UpdateMovementBlendTree()
        {
            animator.SetFloat(_speedAnimatorParameter, navMeshAgent.velocity.sqrMagnitude);
        }
        
        public void LookAtTarget()
        {
            Vector3 lookPos = Target.position - transform.position;
            lookPos.y = 0;
            
            Quaternion desiredRotation = Quaternion.LookRotation(lookPos);

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, enemyStats.RotationRate);
        }

        //is called by enemy animation event
        public void AnimationShootingEvent()
        {
            ShootingAnimationEvent?.Invoke();
        }

        public void TriggerImmediatePathUpdate()
        {
            _updatePathTimer = enemyStats.PathUpdateDelay;
        }
        
        public void UpdatePath()
        {
            _updatePathTimer += Runner.DeltaTime /*Time.deltaTime*/;

            if (_updatePathTimer >= EnemyStats.PathUpdateDelay)
            {
                _updatePathTimer = 0;
                NavMeshAgent.SetDestination(Target.position);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void InstantiateBulletRPC(Vector3 spreadDirection)
        {
            var bullet = Instantiate(PhysicalBulletPrefab, GunMuzzle.position, Quaternion.LookRotation(spreadDirection));
            
            var targetPoint = spreadDirection * 1000f;

            bullet.Initialize(targetPoint, enemyStats.BulletSpeed);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyStats.DetectionDistance);
        }

        public bool CheckIfPlayersOnRadius(out Transform target)
        {
            int numHits = Physics.OverlapSphereNonAlloc(transform.position, EnemyStats.DetectionDistance, _hitColliders, EnemyStats.PlayerLayerMasks);

            if (numHits > 0)
            {
                target = _hitColliders[0].transform;
                return true;
            }
            else
            {
                target = null;
                return false;
            }
        }
    }
    
}
