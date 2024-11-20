using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using Enemy;
using Fusion;
using UnityEngine;

public class EnemyBehaviorDead : State<EnemyBehavior>
{
    public EnemyBehaviorDead(EnemyBehavior context, StateMachine<EnemyBehavior> stateMachine) : base(context, stateMachine)
    {
    }
    
    private readonly int _deathAnimatorParameter = Animator.StringToHash("Dead");

    public override void Enter()
    {
        Debug.Log("<color=purple>Enemy Dead State</color>");

        base.Enter();
        
        context.Collider.enabled = false;
        context.Animator.SetBool(_deathAnimatorParameter, true);
    }

    public override void LogicUpdate()
    {
        
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void Exit()
    {
        base.Exit();
    }
}
