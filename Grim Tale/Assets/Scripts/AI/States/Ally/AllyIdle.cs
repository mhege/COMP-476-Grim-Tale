using UnityEngine;

namespace AI.States.Ally
{
    public class AllyIdle : AllyState
    {
        public AllyIdle(AI.Ally ally) : base(ally)
        {
            name = StateName.Idle;
        }

        public override void Enter()
        {
            ally.Agent.ControlRotation = true;
            ally.Agent.IsStopped = true;
            ally.Animator.SetInteger("State", 0);
            
            base.Enter();
        }

        public override void Update()
        {
            if (ally.Type.Equals(AllyType.Healer))
            {
                nextState = new AllyFollow(ally);
                stage = StateEvent.Exit;

                return;
            }
            
            var playerPosition = ally.Player.transform.position;
            var enemyPosition = ally.transform.position;
            var distanceToPlayer = Vector3.Distance(playerPosition, enemyPosition);
            var directionToPlayer = playerPosition - enemyPosition;

            // Attack
            if (distanceToPlayer < ally.AttackDistance && !Physics.Raycast(enemyPosition + Vector3.up * 0.1f, directionToPlayer, distanceToPlayer, LayerMask.GetMask("Obstacle")))
            {
                nextState = new AllyAttack(ally);
                stage = StateEvent.Exit;

                return;
            }
            
            // Chase
            nextState = new AllyChase(ally);
            stage = StateEvent.Exit;
        }
    }
}
