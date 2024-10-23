using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using Enemy;
using UnityEngine;

namespace Enemy
{
    public class EnemyBehaviorChase : State<EnemyBehavior>
    {
        public EnemyBehaviorChase(EnemyBehavior context, StateMachine<EnemyBehavior> stateMachine) : base(context, stateMachine)
        {
        }

        private float _chaseTime;
        
        public override void Enter()
        {
            base.Enter();
            context.InCombat = true;
            _chaseTime = 0f;
            
            Debug.Log("<color=red>Enemy Chase State</color>");
            context.NavMeshAgent.stoppingDistance = context.EnemyStats.AttackDistance;
            context.TriggerImmediatePathUpdate();
        }

        public override void LogicUpdate()
        {
            _chaseTime += context.Runner.DeltaTime;

            if (_chaseTime >= context.EnemyStats.MaxChaseTime)
            {
                stateMachine.ChangeState(context.BehaviorPatrolState);
                return;
            }
            
            if (Vector3.Distance(context.transform.position, context.Target.position) <=
                context.EnemyStats.AttackDistance)
            {
                stateMachine.ChangeState(context.BehaviorAttackState);
            }
            
            context.LookAtTarget();
            context.UpdatePath();
            context.UpdateMovementBlendTree();
        }

        public override void PhysicsUpdate()
        {
            
        }

        public override void Exit()
        {
            base.Exit();
            context.InCombat = false;
        }
        
    }
}
