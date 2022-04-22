using System.Linq;
using UnityEngine;

namespace AI.States
{
    public class Attack : State
    {
        public Attack(Enemy enemy) : base(enemy)
        {
            name = StateName.Attack;
        }

        public override void Update()
        {
            if (enemy.Type.Equals(EnemyType.GoblinWarchief))
            {
                var allies = Object.FindObjectsOfType<Enemy>().Where(x => x != enemy).ToArray();
                if (allies.Length == 0) return;
                
                var target = allies.OrderBy(x => Vector3.Distance(x.transform.position, enemy.transform.position)).First(); // TODO Take from an eventual GameManager
                if (target)
                {
                    var targetPosition = target.transform.position;
                    var enemyPosition = enemy.transform.position;
                    var distanceToTarget = Vector3.Distance(targetPosition, enemyPosition);
                    var directionToTarget = targetPosition - enemyPosition;
                    
                    if (distanceToTarget > enemy.AttackDistance || Physics.Raycast(enemyPosition + Vector3.up * 0.1f, directionToTarget, distanceToTarget, LayerMask.GetMask("Obstacle")))
                    {
                        if (enemy.HasAnimationAttack)
                        {
                            enemy.Animator.SetTrigger("InterruptAttack");
                        }
                        nextState = new Follow(enemy);
                        stage = StateEvent.Exit;
                
                        return;
                    }
                }
                else
                {
                    nextState = new Follow(enemy);
                    stage = StateEvent.Exit;
                }
            }
            else
            {
                var playerPosition = enemy.Player.transform.position;
                var enemyPosition = enemy.transform.position;
                var distanceToPlayer = Vector3.Distance(playerPosition, enemyPosition);
                var directionToPlayer = playerPosition - enemyPosition;
            
                if (distanceToPlayer > enemy.AttackDistance || Physics.Raycast(enemyPosition + Vector3.up * 0.1f, directionToPlayer, distanceToPlayer, LayerMask.GetMask("Obstacle")))
                {
                    if (enemy.HasAnimationAttack)
                    {
                        enemy.Animator.SetTrigger("InterruptAttack");
                    }
                    nextState = new Chase(enemy);
                    stage = StateEvent.Exit;
                
                    return;
                }
            }
            
            if (!enemy.CanAttack) return;

            if (enemy.HasAnimationAttack)
            {
                enemy.Animator.SetInteger("State", 0);
                enemy.Agent.SetDestination(enemy.transform.position);
                enemy.Agent.IsStopped = true;
                enemy.Agent.ControlRotation = false;

                enemy.Attack();
                enemy.ResetAttackTimer();
            }
            else
            {
                enemy.Agent.SetDestination(enemy.Player.transform.position);
                enemy.Agent.IsStopped = false;
                enemy.Agent.ControlRotation = true;
                enemy.Player.Damage(enemy.Damage);
                enemy.ResetAttackTimer();
            }
        }
    }
}
