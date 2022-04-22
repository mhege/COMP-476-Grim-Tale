namespace AI
{
    public class AllyState
    {
        public StateName name;
        
        protected StateEvent stage;
        protected AllyState nextState;
        protected Ally ally;

        public AllyState(Ally ally)
        {
            stage = StateEvent.Enter;
            this.ally = ally;
        }

        public virtual void Enter() { stage = StateEvent.Update; }
        public virtual void Update() { stage = StateEvent.Update; }
        public virtual void Exit() { stage = StateEvent.Exit; }

        // Processes the next state and stage
        public AllyState Process()
        {
            if (stage == StateEvent.Enter) Enter();
            if (stage == StateEvent.Update) Update();
            if(stage == StateEvent.Exit)
            {
                Exit();
                return nextState;
            }

            return this;
        }
    }
}