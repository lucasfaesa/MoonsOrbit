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

        public override void Enter()
        {
            base.Enter();
            Debug.Log("<color=red>Enemy Chase State</color>");
            context.NavMeshAgent.stoppingDistance = context.EnemyStats.AttackDistance;
            context.TriggerImmediatePathUpdate();
        }

        public override void LogicUpdate()
        {
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
        }
        
    }
}
