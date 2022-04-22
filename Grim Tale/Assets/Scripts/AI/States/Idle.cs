using UnityEngine;

namespace AI.States
{
    public class Idle : State
    {
        public Idle(Enemy enemy) : base(enemy)
        {
            name = StateName.Idle;
        }

        public override void Enter()
        {
            enemy.Agent.ControlRotation = true;
            enemy.Agent.IsStopped = true;
            enemy.Animator.SetInteger("State", 0);
            
            base.Enter();
        }

        public override void Update()
        {
            if (enemy.Type.Equals(EnemyType.GoblinWarchief))
            {
                nextState = new Follow(enemy);
                stage = StateEvent.Exit;

                return;
            }
            
            var playerPosition = enemy.Player.transform.position;
            var enemyPosition = enemy.transform.position;
            var distanceToPlayer = Vector3.Distance(playerPosition, enemyPosition);
            var directionToPlayer = playerPosition - enemyPosition;
            
            // Formation
            if (distanceToPlayer > enemy.ChaseDistance && formationPoint is {Ready: true} && enemy.Type != enemy.UpgradeType)
            {
                nextState = new Formation(enemy);
                stage = StateEvent.Exit;

                return;
            }
            
            // Attack
            if (distanceToPlayer < enemy.AttackDistance && !Physics.Raycast(enemyPosition + Vector3.up * 0.1f, directionToPlayer, distanceToPlayer, LayerMask.GetMask("Obstacle")))
            {
                nextState = new Attack(enemy);
                stage = StateEvent.Exit;

                return;
            }
            
            // Chase
            nextState = new Chase(enemy);
            stage = StateEvent.Exit;
        }
    }
}
