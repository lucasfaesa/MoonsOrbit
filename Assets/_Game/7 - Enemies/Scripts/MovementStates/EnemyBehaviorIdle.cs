using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;


namespace Enemy
{
    public class EnemyBehaviorIdle : State<EnemyBehavior>
    {
        public EnemyBehaviorIdle(EnemyBehavior context, StateMachine<EnemyBehavior> stateMachine) : base(context, stateMachine)
        {
        }

        private float _timer;
        
        public override void Enter()
        {
            Debug.Log("<color=yellow>Enemy Idle State</color>");
            base.Enter();
            _timer = 0;
        }

        public override void LogicUpdate()
        {
            if (context.CheckIfPlayersOnRadius(out var target))
            {
                context.Target = target;
                stateMachine.ChangeState(context.BehaviorAttackState);
            }
            
            _timer += Time.deltaTime;

            if (_timer >= context.EnemyStats.IdleTime)
            {
                _timer = 0;
                stateMachine.ChangeState(context.BehaviorPatrolState);
            }
            

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
