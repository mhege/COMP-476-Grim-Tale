using UnityEngine;

namespace AI.States
{
    public class Charge : State
    {
        private Vector3 chargeDirection;
        
        public Charge(Enemy enemy) : base(enemy)
        {
            name = StateName.Charge;
        }
        
        public override void Enter()
        {
            enemy.Agent.MaximumSpeed = 8f;
            enemy.Agent.ControlRotation = true;
            enemy.Agent.IsStopped = false;

            enemy.Animator.SetInteger("State", 2);

            var playerPosition = enemy.Player.transform.position;
            var enemyPosition = enemy.transform.position + Vector3.up * 0.1f;
            var adjustedPlayerPosition = new Vector3(playerPosition.x, enemyPosition.y, playerPosition.z);
            chargeDirection = adjustedPlayerPosition - enemyPosition;
            
            if (!Physics.Raycast(enemyPosition, chargeDirection, out var hitInfo, float.PositiveInfinity, LayerMask.GetMask("Obstacle"))) return;
            
            enemy.Agent.SetDestination(hitInfo.point);
            
            base.Enter();
        }

        public Vector3 ChargeDirection => chargeDirection;
    }
}
