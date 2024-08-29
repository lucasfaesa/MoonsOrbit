using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

public class CombatFightState : State<PlayerCombatBehavior>
{
    public CombatFightState(PlayerCombatBehavior context, StateMachine<PlayerCombatBehavior> stateMachine) : base(context, stateMachine)
    {
    }
    
    private float _lastShotTime = 0f;
    
    public override void Enter()
    {
        Debug.Log("<color=magenta>Entered combat fight state</color>");
    }

    public override void LogicUpdate()
    {
        if (context.Runner.LocalRenderTime - _lastShotTime > context.PistolStats.DelayBetweenShots)
        {
            _lastShotTime = context.Runner.LocalRenderTime;

            var bullet = context.Runner.Spawn(context.BulletPrefab, context.BulletRef.position, context.BulletRef.rotation);
            bullet.gameObject.SetActive(true);
            
        }

        if (!context.InputReader.ShootStatus)
            stateMachine.ChangeState(context.CombatIdleState);
    }

    public override void PhysicsUpdate()
    {

    }

    public override void Exit()
    {

    }
}
