using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using Enemy;
using UnityEngine;

public class EnemyBehaviorPatrol : State<EnemyBehavior>
{
    public EnemyBehaviorPatrol(EnemyBehavior context, StateMachine<EnemyBehavior> stateMachine) : base(context, stateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log("<color=cyan>Enemy Patrol State</color>");
        base.Enter();
    }

    public override void LogicUpdate()
    {
        //context.UpdateMovementBlendTree();
        
        if (Vector3.Distance(context.transform.position, context.Target.position) <=
            context.EnemyStats.DetectionDistance)
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
    }

}
