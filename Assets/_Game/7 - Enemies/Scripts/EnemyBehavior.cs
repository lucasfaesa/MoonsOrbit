using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DesignPatterns;
using DG.Tweening;
using Fusion;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;


namespace Enemy
{
    public class EnemyBehavior : NetworkBehaviour, IStateAuthorityChanged
    {

        [Header("SOs")]
        [SerializeField] private EnemyStatsSO enemyStats;
        [SerializeField] private HealthStatsSO healthStats;
        [Header("Animation Rig Related")] 
        [SerializeField] private Transform weaponTransform;
        [SerializeField] private Transform rigAimTarget;
        [SerializeField] private MultiAimConstraint bodyAimConstraint;
        [SerializeField] private MultiAimConstraint aimConstraint;
        [SerializeField] private MultiAimConstraint neckConstraint;
        [Header("References")] 
        [SerializeField] private NetworkObject networkObject;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform gunMuzzle;
        [SerializeField] private ParticleSystem muzzleFlashParticle;
        [SerializeField] private PhysicalBulletBehavior physicalBulletPrefab;
        [SerializeField] private Collider collider;
        [Header("Patrol Points")] 
        [SerializeField] private Transform patrolLocations;
        [SerializeField] private List<Transform> patrolPoints = new();
        
        public Transform Target { get; set; }
        [Networked] public bool InCombat { get; set; }
        [Networked, OnChangedRender(nameof(SetRigWeights))] public bool Attacking { get; set; }
        public EnemyStatsSO EnemyStats => enemyStats;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public Animator Animator => animator;
        public Transform GunMuzzle => gunMuzzle;
        public ParticleSystem MuzzleFlashParticle => muzzleFlashParticle;
        public PhysicalBulletBehavior PhysicalBulletPrefab => physicalBulletPrefab;
        public List<Transform> PatrolPoints => patrolPoints;
        public Collider Collider => collider;
        
        public event Action ShootingAnimationEvent;
        
        //----- State Machine things -----
        public EnemyBehaviorIdle BehaviorIdleState { get; private set; }
        public EnemyBehaviorPatrol BehaviorPatrolState { get; private set; }
        public EnemyBehaviorChase BehaviorChaseState { get; private set; }
        public EnemyBehaviorAttack BehaviorAttackState { get; private set; }
        public EnemyBehaviorDead BehaviorDeadState { get; private set; }
        
        private StateMachine<EnemyBehavior> _stateMachine = new();
        //---------
        
        private readonly int _speedAnimatorParameter = Animator.StringToHash("Speed");
        private float _updatePathTimer = 0;
        private Collider[] _hitColliders = new Collider[10];

        private HashSet<AttackerData> _attackers = new();

        public void IsPreparingToChangeStateAuthority()
        {
            Debug.LogError("Preparing to change state authority");
            
            InCombat = false;
            Attacking = false;
            Target = null;
        }
        
        public void StateAuthorityChanged()
        {
            if (Object.HasStateAuthority)
            {
                Debug.LogError("I have state authority");

                InCombat = false;
                Attacking = false;
                Target = null;
                
                Spawned();
            }
        }
        
        public override void Spawned()
        {
            base.Spawned();
            
            patrolLocations.SetParent(null);
            
            if (!HasStateAuthority)
            {
                navMeshAgent.enabled = false;
                return;
            }

            Reset();
            
            healthStats.GotAttacked += OnGotAttacked;
            healthStats.Death += OnDeath;
            
            navMeshAgent.enabled = true;
            
            BehaviorIdleState = new EnemyBehaviorIdle(this, _stateMachine);
            BehaviorPatrolState = new EnemyBehaviorPatrol(this, _stateMachine);
            BehaviorChaseState = new EnemyBehaviorChase(this, _stateMachine);
            BehaviorAttackState = new EnemyBehaviorAttack(this, _stateMachine);
            BehaviorDeadState = new EnemyBehaviorDead(this, _stateMachine);
            
            _stateMachine.Initialize(BehaviorIdleState);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            healthStats.GotAttacked -= OnGotAttacked;
            healthStats.Death -= OnDeath;
        }

        public void UnsubscribeFromEvents()
        {
            healthStats.GotAttacked -= OnGotAttacked;
            healthStats.Death -= OnDeath;
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            
            if (!HasStateAuthority)
                return;
            
            if (InCombat)
                rigAimTarget.position = Target.position;
            if (Attacking)
                weaponTransform.LookAt(Target.position);
            
            _stateMachine.Update();
        }
        
        public void UpdateMovementBlendTree()
        {
            animator.SetFloat(_speedAnimatorParameter, navMeshAgent.velocity.sqrMagnitude);
        }

        private void OnDeath(uint networkId)
        {
            if (networkObject.Id.Raw != networkId)
                return;
            
            navMeshAgent.isStopped = true;
            
            _stateMachine.ChangeState(BehaviorDeadState);
        }

        private void OnGotAttacked(Vector3 attackerPosition, uint networkId)
        {
            if (networkObject.Id.Raw != networkId)
                return;
            
            Collider[] hits = new Collider[1];
            int hitCount = Physics.OverlapSphereNonAlloc(attackerPosition, 1f, hits, enemyStats.PlayerLayerMasks);
            
            if (hitCount == 0)
                return;
            
            var attacker = hits[0].transform;
            
            if (!InCombat)
            {
                InCombat = true;
                Target = attacker;
                _stateMachine.ChangeState(BehaviorChaseState);
            }
            
            var attackerData = _attackers.FirstOrDefault(x => x.AttackerTransform == attacker);
            
            if (attackerData == null)
                _attackers.Add(new AttackerData(attacker, 1, Runner.LocalRenderTime)); // Add new attacker if it doesn't exist
            else
            {
                attackerData.DamageDone++;
                attackerData.LastDamageTime = Runner.LocalRenderTime;
            }
            
            
            // If multiple attackers, handle target switching
            if (_attackers.Count > 1)
            {
                var currentTargetData = _attackers.FirstOrDefault(x => x.AttackerTransform == Target);

                if (currentTargetData != null)
                {
                    float timeSinceLastAttack = Runner.LocalRenderTime - currentTargetData.LastDamageTime;
                    float damageDifference = attackerData != null ? attackerData.DamageDone - currentTargetData.DamageDone : 0;

                    // Switch target if the current target has not attacked for a certain delay
                    if (timeSinceLastAttack >= enemyStats.TargetChangeDelayWhenBeingAttacked)
                    {
                        Target = attacker;
                    }

                    // Switch target if there's a significant difference in damage taken
                    if (damageDifference >= enemyStats.BulletsTakenThresholdToChangeTarget)
                    {
                        Target = attacker;
                    }
                }
            }
        }
        
        private void SetRigWeights()
        {
            float targetWeight = Attacking ? 1f : 0f;

            // Smoothly transition each weight to the target value over one second
            DOTween.To(() => bodyAimConstraint.weight, x => bodyAimConstraint.weight = x, targetWeight, 0.5f);
            DOTween.To(() => aimConstraint.weight, x => aimConstraint.weight = x, targetWeight, 0.5f);
            DOTween.To(() => neckConstraint.weight, x => neckConstraint.weight = x, targetWeight, 0.5f);
        
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

            bullet.Initialize(targetPoint, enemyStats.BulletSpeed, enemyStats.PerBulletDamage);
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

        private class AttackerData
        {
            public Transform AttackerTransform { get; }
            public float DamageDone { get; set; }
            public float LastDamageTime { get; set; }

            public AttackerData(Transform attackerTransform, float damageDone, float lastDamageTime)
            {
                AttackerTransform = attackerTransform;
                DamageDone = damageDone;
                LastDamageTime = lastDamageTime;
            }
        }
        
        
        private void Reset()
        {
            navMeshAgent.isStopped = false;
            InCombat = false;
            Attacking = false;
            Collider.enabled = true;
        }
    }
    
}
