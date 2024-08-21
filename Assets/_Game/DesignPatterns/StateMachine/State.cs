

namespace DesignPatterns
{
    public abstract class State<T>
    {
        protected T context;
        protected StateMachine<T> stateMachine;

        protected State(T context, StateMachine<T> stateMachine)
        {
            this.context = context;
            this.stateMachine = stateMachine;
        }

        public abstract void Enter();
        public abstract void LogicUpdate();
        public abstract void PhysicsUpdate();
        public abstract void Exit();
    } 
}

