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
        
        //TODO remove this atrocity later
        [field:SerializeField]public Transform Target { get; set; }

        public EnemyStatsSO EnemyStats => enemyStats;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public Animator Animator => animator;
        
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

    }
    
}
