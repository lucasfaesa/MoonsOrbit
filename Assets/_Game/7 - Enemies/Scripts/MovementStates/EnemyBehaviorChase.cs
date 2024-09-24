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

        private float _timer = 0;


        public override void Enter()
        {
            base.Enter();
            Debug.Log("<color=red>Enemy Chase State</color>");
            
            //just for it to update the path right when it enters this state, hehe
            _timer = context.EnemyStats.PathUpdateDelay;
        }

        public override void LogicUpdate()
        {
            if (Vector3.Distance(context.transform.position, context.Target.position) <=
                context.EnemyStats.AttackDistance)
            {
                stateMachine.ChangeState(context.BehaviorAttackState);
            }
            
            LookAtTarget();
            UpdatePath();
            context.UpdateMovementBlendTree();
        }

        public override void PhysicsUpdate()
        {
            
        }

        public override void Exit()
        {
            base.Exit();
        }

        private void LookAtTarget()
        {
            Vector3 lookPos = context.Target.position - context.transform.position;
            lookPos.y = 0;
            
            Quaternion desiredRotation = Quaternion.LookRotation(lookPos);

            context.transform.rotation = Quaternion.Slerp(context.transform.rotation, desiredRotation, 0.2f);
        }

        private void UpdatePath()
        {
            _timer += Time.deltaTime;

            if (_timer >= context.EnemyStats.PathUpdateDelay)
            {
                _timer = 0;
                context.NavMeshAgent.SetDestination(context.Target.position);
            }
        }
    }
}
