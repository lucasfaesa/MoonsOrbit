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

    private int _targetPatrolPointIndex;
    private bool _inRestTimeBetweenWaypoints;
    private float _restTimeBetweenPatrolPoints = 3f;
    private float _isMoving;
    
    private Coroutine _waitToChangeTargetPatrolPoint;
    
    public override void Enter()
    {
        _inRestTimeBetweenWaypoints = false;

        context.NavMeshAgent.stoppingDistance = 0;
        _targetPatrolPointIndex = 0;
        context.Target = context.PatrolPoints[_targetPatrolPointIndex];
        
        context.TriggerImmediatePathUpdate();
        
        Debug.Log("<color=cyan>Enemy Patrol State</color>");
        base.Enter();
    }

    public override void LogicUpdate()
    {
        context.UpdateMovementBlendTree();

        if (context.CheckIfPlayersOnRadius(out var target))
        {
            context.Target = target;
            stateMachine.ChangeState(context.BehaviorAttackState);
        }

        
        if (!_inRestTimeBetweenWaypoints)
        {
            context.UpdatePath();
            CheckIfReachedDestination();
        }
    }

    private void CheckIfReachedDestination()
    {
        if (_inRestTimeBetweenWaypoints || context.NavMeshAgent.pathPending) return;

        if (context.NavMeshAgent.remainingDistance <= context.NavMeshAgent.stoppingDistance 
                && context.NavMeshAgent.velocity.sqrMagnitude == 0f)
        {
            
            _inRestTimeBetweenWaypoints = true;
            _waitToChangeTargetPatrolPoint = context.StartCoroutine(WaitAndChangeTargetPatrolPoint());
        }
        
    }

    private IEnumerator WaitAndChangeTargetPatrolPoint()
    {
        yield return new WaitForSeconds(_restTimeBetweenPatrolPoints);
        
        //cycling through
        _targetPatrolPointIndex = (_targetPatrolPointIndex + 1) % context.PatrolPoints.Count;
        
        context.Target = context.PatrolPoints[_targetPatrolPointIndex];

        _inRestTimeBetweenWaypoints = false;
        context.TriggerImmediatePathUpdate();
    }
    
    public override void PhysicsUpdate()
    {
                
    }

    public override void Exit()
    {
        base.Exit();
        if(_waitToChangeTargetPatrolPoint != null)
            context.StopCoroutine(_waitToChangeTargetPatrolPoint);
    }

}
