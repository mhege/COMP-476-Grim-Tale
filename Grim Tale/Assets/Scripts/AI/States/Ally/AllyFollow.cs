using System.Linq;
using UnityEngine;

namespace AI.States.Ally
{
    public class AllyFollow : AllyState
    {
        public AllyFollow(AI.Ally ally) : base(ally)
        {
            name = StateName.Follow;
        }
        
        public override void Enter()
        {
            ally.Agent.IsStopped = false;
            ally.Agent.ControlRotation = true;
            ally.Agent.MaximumSpeed = GetTypeSpeed();
            
            var random = GetAnimationVariations();
            ally.Animator.SetInteger("Random", random);
            ally.Animator.SetInteger("State", 1);

            if (ally.HasAnimationAttack)
            {
                ally.Animator.ResetTrigger("InterruptAttack");
            }
            
            base.Enter();
        }

        public override void Update()
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
            
            // Attack
            if (distanceToTarget < ally.AttackDistance && !Physics.Raycast(allyPosition + Vector3.up * 0.1f, directionToTarget, distanceToTarget, LayerMask.GetMask("Obstacle")))
            {
                nextState = new AllyAttack(ally);
                stage = StateEvent.Exit;

                return;
            }

            // Chase
            ally.Agent.SetDestination(ally.Player.IsDamaged() || !target ? ally.Player.transform.position : target.transform.position);
        }

        public override void Exit()
        {
            ally.Agent.FormationVector = Vector3.zero;
            
            base.Exit();
        }

        private float GetTypeSpeed()
        {
            switch (ally.Type)
            {
                case AllyType.Healer:
                    return 4f;
                case AllyType.Sorcerer:
                    return 4f;
                default:
                    return 4f;
            }
        }

        private int GetAnimationVariations()
        {
            switch (ally.Type)
            {
                case AllyType.Healer:
                    return Random.Range(0, 1);
                case AllyType.Sorcerer:
                    return Random.Range(0, 1);
                default:
                    return Random.Range(0, 1);
            }
        }
    }
}
