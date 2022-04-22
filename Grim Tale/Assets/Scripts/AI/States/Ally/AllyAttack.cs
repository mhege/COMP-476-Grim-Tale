using System.Linq;
using UnityEngine;

namespace AI.States.Ally
{
    public class AllyAttack : AllyState
    {
        public AllyAttack(AI.Ally ally) : base(ally)
        {
            name = StateName.Attack;
        }

        public override void Update()
        {
            if (ally.Type.Equals(AllyType.Healer))
            {
                var allies = Object.FindObjectsOfType<AI.Ally>().Where(x => x != ally).ToArray();
                AI.Ally target = null;
                if (allies.Length != 0)
                {
                    target = allies.OrderBy(x => Vector3.Distance(x.transform.position, ally.transform.position)).First(); // TODO Take from an eventual GameManager
                }
                
                var targetPosition = ally.Player.IsDamaged() || !target ? ally.Player.transform.position : target.transform.position;
                var allyPosition = ally.transform.position;
                var distanceToTarget = Vector3.Distance(targetPosition, allyPosition);
                var directionToTarget = targetPosition - allyPosition;
                
                if (distanceToTarget > ally.AttackDistance || Physics.Raycast(allyPosition + Vector3.up * 0.1f, directionToTarget, distanceToTarget, LayerMask.GetMask("Obstacle")))
                {
                    if (ally.HasAnimationAttack)
                    {
                        ally.Animator.SetTrigger("InterruptAttack");
                    }
                    nextState = new AllyFollow(ally);
                    stage = StateEvent.Exit;
            
                    return;
                }
            }
            else
            {
                var playerPosition = ally.Player.transform.position;
                var enemyPosition = ally.transform.position;
                var distanceToPlayer = Vector3.Distance(playerPosition, enemyPosition);
                var directionToPlayer = playerPosition - enemyPosition;
            
                if (distanceToPlayer > ally.AttackDistance || Physics.Raycast(enemyPosition + Vector3.up * 0.1f, directionToPlayer, distanceToPlayer, LayerMask.GetMask("Obstacle")))
                {
                    if (ally.HasAnimationAttack)
                    {
                        ally.Animator.SetTrigger("InterruptAttack");
                    }
                    nextState = new AllyChase(ally);
                    stage = StateEvent.Exit;
                
                    return;
                }
            }
            
            if (!ally.CanAttack) return;

            if (ally.HasAnimationAttack)
            {
                ally.Animator.SetInteger("State", 0);
                ally.Agent.SetDestination(ally.transform.position);
                ally.Agent.IsStopped = true;
                ally.Agent.ControlRotation = false;

                ally.Attack();
                ally.ResetAttackTimer();
            }
            else
            {
                ally.Agent.SetDestination(ally.Player.transform.position);
                ally.Agent.IsStopped = false;
                ally.Agent.ControlRotation = true;
                ally.Player.Damage(ally.Damage);
                ally.ResetAttackTimer();
            }
        }
    }
}
