using UnityEngine;
using AI.Pathfinding;
using AI.States.Ally;

namespace AI
{
    public class Ally : MonoBehaviour, IHealable
    {
        [Header("Ally")]
        [SerializeField] private float attackDistance = 2f;
        [SerializeField] protected int damage = 1;
        [SerializeField] private float damageCooldown = 1f;
        [SerializeField] protected float maxHealth = 50f;
        [SerializeField] protected float health = 50f;
        [SerializeField] protected float damageByLightAttack = 10f;
        [SerializeField] protected float damageByHeavyAttack = 30f;
        [SerializeField] private bool hasAnimationAttack;
        [SerializeField] private AllyType type;
        [SerializeField] protected ParticleSystem killParticleSystem;
        [SerializeField] protected ParticleSystem damageParticleSystem;

        protected PathfindingAgent agent;
        protected AllyState state;
        protected Animator animator;
        protected bool stateBlocked;
        protected PlayerController player;
        
        private float attackTimer;
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<PathfindingAgent>();
            player = FindObjectOfType<PlayerController>();
        }

        private void Start()
        {
            state = new AllyIdle(this);
        }
        
        protected virtual void Update()
        {
            if(!stateBlocked)
                state = state?.Process();

            attackTimer -= Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("LightProjectile"))
            {
                var positionVector = other.transform.position;
                Instantiate(damageParticleSystem, positionVector, transform.rotation);
                Destroy(other.gameObject);
                health -= damageByLightAttack;

                if (health <= 0)
                {
                    Instantiate(killParticleSystem, positionVector, transform.rotation);
                    Destroy(gameObject);
                }
            }
            else if (other.transform.CompareTag("HeavyProjectile"))
            {
                var positionVector = other.transform.position;
                Instantiate(damageParticleSystem, positionVector, transform.rotation);
                Destroy(other.gameObject);
                health -= damageByHeavyAttack;

                if (health <= 0)
                {
                    Instantiate(killParticleSystem, positionVector, transform.rotation);
                    Destroy(gameObject);
                }
            }
        }
        
        public void Heal(int amount)
        {
            health = Mathf.Min(health + amount, maxHealth);
        }

        public virtual void Attack()
        {
            player.Damage(damage);
        }

        public void DealDamage(int amount)
        {
            var positionVector = transform.position;
            Instantiate(damageParticleSystem, positionVector, transform.rotation);
            health -= damageByHeavyAttack;
            
            health -= damage;
            if (health <= 0)
            {
                Instantiate(killParticleSystem, positionVector, transform.rotation);
                Destroy(gameObject);
            }
        }

        public void ResetAttackTimer()
        {
            attackTimer = damageCooldown;
        }

        public Animator Animator => animator;
        public PathfindingAgent Agent => agent;
        public PlayerController Player => player;
        public float AttackDistance => attackDistance;
        public int Damage => damage;
        public bool HasAnimationAttack => hasAnimationAttack;
        public AllyType Type => type;
        public bool CanAttack => attackTimer < 0f;
        public bool StateBlocked { set => stateBlocked = value; }
    }
}

public enum AllyType { Healer, Sorcerer }