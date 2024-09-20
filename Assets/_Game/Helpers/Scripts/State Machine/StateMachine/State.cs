

using Unity.VisualScripting;

namespace DesignPatterns
{
    public abstract class State<T>
    {
        protected T context;
        protected StateMachine<T> stateMachine;

        protected bool _isThisStateExiting;
        
        protected State(T context, StateMachine<T> stateMachine)
        {
            this.context = context;
            this.stateMachine = stateMachine;
        }

        public virtual void Enter()
        {
            _isThisStateExiting = false;
        }

        public abstract void LogicUpdate();

        public abstract void PhysicsUpdate();

        public virtual void Exit()
        {
            _isThisStateExiting = true;
        }
    } 
}

