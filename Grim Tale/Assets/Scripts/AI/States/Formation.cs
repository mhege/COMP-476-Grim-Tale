using UnityEngine;

namespace AI.States
{
    public class Formation : State
    {
        public Formation(Enemy enemy) : base(enemy)
        {
            name = StateName.Formation;
        }

        public override void Enter()
        {
            enemy.Agent.IsStopped = false;
            enemy.Agent.ControlRotation = true;

            var random = Random.Range(0, 2);
            enemy.Animator.SetInteger("Random", random);
            enemy.Animator.SetInteger("State", 1);
            
            base.Enter();
        }
        
        public override void Update()
        {
            var distanceToPlayer = Vector3.Distance(enemy.Player.transform.position, enemy.transform.position);
            if (distanceToPlayer <= enemy.ChaseDistance || !formationPoint.Ready)
            {
                nextState = new Chase(enemy);
                stage = StateEvent.Exit;
            }
                        
            enemy.Agent.SetDestination(formationPoint.transform.position);
        }
    }
}
