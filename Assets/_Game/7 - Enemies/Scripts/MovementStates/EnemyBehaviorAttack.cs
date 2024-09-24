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
            context.Animator.SetBool(_shootingAnimatorParameter, true);
        }

        public override void LogicUpdate()
        {
            if (Vector3.Distance(context.transform.position, context.Target.position) >=
                context.EnemyStats.AttackDistance)
            {
                stateMachine.ChangeState(context.BehaviorChaseState);
            }
        }

        public override void PhysicsUpdate()
        {
            
        }

        public override void Exit()
        {
            base.Exit();
            context.Animator.SetBool(_shootingAnimatorParameter, false);
        }
    }
}
