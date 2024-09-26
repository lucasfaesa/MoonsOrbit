using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;


namespace Enemy
{
    public class EnemyBehavior : MonoBehaviour
    {

        [Header("SOs")]
        [SerializeField] private EnemyStatsSO enemyStats;
        [Header("References")] 
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform gunMuzzle;
        [SerializeField] private ParticleSystem muzzleFlashParticle;
        [SerializeField] private PhysicalBulletBehavior physicalBulletPrefab;
        
        //TODO remove this atrocity later
        [field:SerializeField] public Transform Target { get; set; }

        public EnemyStatsSO EnemyStats => enemyStats;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public Animator Animator => animator;
        public Transform GunMuzzle => gunMuzzle;
        public ParticleSystem MuzzleFlashParticle => muzzleFlashParticle;
        public PhysicalBulletBehavior PhysicalBulletPrefab => physicalBulletPrefab;

        public event Action ShootingAnimationEvent;
        
        //----- State Machine things -----
        public EnemyBehaviorIdle BehaviorIdleState { get; private set; }
        public EnemyBehaviorPatrol BehaviorPatrolState { get; private set; }
        public EnemyBehaviorChase BehaviorChaseState { get; private set; }
        public EnemyBehaviorAttack BehaviorAttackState { get; private set; }
        
        private StateMachine<EnemyBehavior> _stateMachine = new();
        //---------
        
        private readonly int _speedAnimatorParameter = Animator.StringToHash("Speed");
        
        void Start()
        {
            navMeshAgent.stoppingDistance = enemyStats.AttackDistance;
            
            BehaviorIdleState = new EnemyBehaviorIdle(this, _stateMachine);
            BehaviorPatrolState = new EnemyBehaviorPatrol(this, _stateMachine);
            BehaviorChaseState = new EnemyBehaviorChase(this, _stateMachine);
            BehaviorAttackState = new EnemyBehaviorAttack(this, _stateMachine);
            
            _stateMachine.Initialize(BehaviorIdleState);
        }

        void Update()
        {
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

    }
    
}
