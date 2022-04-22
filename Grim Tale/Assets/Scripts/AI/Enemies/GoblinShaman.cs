using System.Collections;
using UnityEngine;

namespace AI.Enemies
{
    public class GoblinShaman : Enemy
    {
        [Header("Goblin Shaman")]
        [SerializeField] private EnemyProjectile enemyProjectilePrefab;
        [SerializeField] private Transform projectileSpawnPosition;

        public override void Attack()
        {
            StartCoroutine(RotateTowardsTarget());
        }

        private IEnumerator RotateTowardsTarget()
        {
            var position = transform.position;
            var targetPosition = player.transform.position;
            var adjustedTargetPosition = new Vector3(targetPosition.x, position.y, targetPosition.z);
            var targetLookRotation = adjustedTargetPosition - position;
            do
            {
                var lookRotation = Quaternion.LookRotation(targetLookRotation, Vector3.up);
                var lerpRotation = Quaternion.Lerp(transform.rotation, lookRotation, 20f * Time.deltaTime);
                transform.rotation = lerpRotation;

                yield return null;
            } 
            while (Vector3.Angle(targetLookRotation, transform.forward) > 2f);
            
            animator.SetTrigger("Attack");
        }
        
        // Animation event
        private void InstantiateProjectile()
        {
            var position = transform.position;
            var targetPosition = player.transform.position;
            var adjustedTargetPosition = new Vector3(targetPosition.x, position.y, targetPosition.z);
            var direction = adjustedTargetPosition - position;
            
            var projectile = Instantiate(enemyProjectilePrefab, projectileSpawnPosition.position, projectileSpawnPosition.rotation);
            projectile.SetParameters(damage, direction);
        }
    }
}
