using UnityEngine;

namespace AI
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private float travelSpeed = 8f;

        private int damage;
        private Vector3 direction;

        private void Update()
        {
            transform.Translate(direction * Time.deltaTime * travelSpeed, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out var player))
            {
                player.Damage(damage);
            }

            if (other.TryGetComponent<Ally>(out var ally))
            {
                ally.DealDamage(damage);
            }
        
            Destroy(gameObject);
        }

        public void SetParameters(int value, Vector3 direction)
        {
            damage = value;
            this.direction = direction.normalized;
        }
    }
}
