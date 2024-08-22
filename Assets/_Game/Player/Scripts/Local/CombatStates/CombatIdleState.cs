using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

public class CombatIdleState : State<PlayerCombatBehavior>
{
    public CombatIdleState(PlayerCombatBehavior context, StateMachine<PlayerCombatBehavior> stateMachine) : base(context, stateMachine)
    {
    }

    public override void Enter()
    {

    }

    public override void LogicUpdate()
    {

    }

    public override void PhysicsUpdate()
    {

    }

    public override void Exit()
    {

    }
}
