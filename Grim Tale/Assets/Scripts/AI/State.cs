namespace AI
{
    public class State
    {
        public StateName name;
        
        protected StateEvent stage;
        protected State nextState;
        protected Enemy enemy;
        protected FormationPoint formationPoint;

        public State(Enemy enemy)
        {
            stage = StateEvent.Enter;
            this.enemy = enemy;
        }

        public virtual void Enter()
        {
            formationPoint = FormationsManager.Instance.GetFormationPoint(enemy.Type);
            stage = StateEvent.Update;
        }
        public virtual void Update() { stage = StateEvent.Update; }
        public virtual void Exit() { stage = StateEvent.Exit; }

        // Processes the next state and stage
        public State Process()
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

    public enum StateName { Idle, Chase, Formation, Attack, Charge, Follow }
    public enum StateEvent { Enter, Update, Exit }
}