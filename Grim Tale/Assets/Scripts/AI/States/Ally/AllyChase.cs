using UnityEngine;

namespace AI.States.Ally
{
    public class AllyChase : AllyState
    {
        public AllyChase(AI.Ally ally) : base(ally)
        {
            name = StateName.Chase;
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
            ally.Agent.SetDestination(ally.Player.transform.position);
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
