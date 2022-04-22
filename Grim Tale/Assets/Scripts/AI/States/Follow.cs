using System.Linq;
using UnityEngine;

namespace AI.States
{
    public class Follow : State
    {
        public Follow(Enemy enemy) : base(enemy)
        {
            name = StateName.Follow;
        }
        
        public override void Enter()
        {
            enemy.Agent.IsStopped = false;
            enemy.Agent.ControlRotation = true;
            enemy.Agent.MaximumSpeed = GetTypeSpeed();
            
            var random = GetAnimationVariations();
            enemy.Animator.SetInteger("Random", random);
            enemy.Animator.SetInteger("State", 1);

            if (enemy.HasAnimationAttack)
            {
                enemy.Animator.ResetTrigger("InterruptAttack");
            }
            
            base.Enter();
        }

        public override void Update()
        {
            var allies = Object.FindObjectsOfType<Enemy>().Where(x => x != enemy).ToArray();
            if (allies.Length == 0) return;
            
            var target = allies.OrderBy(x => Vector3.Distance(x.transform.position, enemy.transform.position)).First(); // TODO Take from an eventual GameManager
            if (!target) return; // TODO Go to idle?
            
            var targetPosition = target.transform.position;
            var enemyPosition = enemy.transform.position;
            var distanceToTarget = Vector3.Distance(targetPosition, enemyPosition);
            var directionToTarget = targetPosition - enemyPosition;
            
            // Attack
            if (distanceToTarget < enemy.AttackDistance && !Physics.Raycast(enemyPosition + Vector3.up * 0.1f, directionToTarget, distanceToTarget, LayerMask.GetMask("Obstacle")))
            {
                nextState = new Attack(enemy);
                stage = StateEvent.Exit;

                return;
            }

            // Chase
            enemy.Agent.SetDestination(target.transform.position);
        }

        public override void Exit()
        {
            enemy.Agent.FormationVector = Vector3.zero;
            
            base.Exit();
        }

        private float GetTypeSpeed()
        {
            switch (enemy.Type)
            {
                case EnemyType.Skeleton:
                    return 1.5f;
                case EnemyType.MutantCharger:
                    return 1.5f;
                case EnemyType.GoblinShaman:
                    return 4f;
                case EnemyType.GoblinWarchief:
                    return 4f;
                default:
                    return 1.5f;
            }
        }

        private int GetAnimationVariations()
        {
            switch (enemy.Type)
            {
                case EnemyType.Skeleton:
                    return Random.Range(0, 2);
                case EnemyType.MutantCharger:
                    return Random.Range(0, 2);
                case EnemyType.GoblinShaman:
                    return Random.Range(0, 1);
                case EnemyType.GoblinWarchief:
                    return Random.Range(0, 1);
                default:
                    return Random.Range(0, 1);
            }
        }
    }
}
