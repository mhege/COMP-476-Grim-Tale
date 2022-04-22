using System;
using UnityEngine;
using AI.Pathfinding;
using AI.States;

namespace AI
{
    public class Enemy : MonoBehaviour
    {
        [Header("Enemy")]
        [SerializeField] private float chaseDistance = 20f;
        [SerializeField] private float attackDistance = 2f;
        [SerializeField] protected int damage = 1;
        [SerializeField] private float damageCooldown = 1f;
        [SerializeField] protected float health = 50f;
        [SerializeField] protected float maxHealth = 50f;
        [SerializeField] protected float damageByLightAttack = 10f;
        [SerializeField] protected float damageByHeavyAttack = 30f;
        [SerializeField] private bool hasAnimationAttack;
        [SerializeField] private EnemyType type;
        [SerializeField] private EnemyType upgradeType;
        [SerializeField] protected ParticleSystem killParticleSystem;
        [SerializeField] protected ParticleSystem damageParticleSystem;

        protected PathfindingAgent agent;
        protected State state;
        protected Animator animator;
        protected bool stateBlocked;
        protected PlayerController player;
        
        private float attackTimer;

        private float re = .75f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<PathfindingAgent>();
            player = FindObjectOfType<PlayerController>();
        }

        private void Start()
        {
            state = new Idle(this);
        }
        
        protected virtual void Update()
        {
            if(!stateBlocked)
                state = state?.Process();

            attackTimer -= Time.deltaTime;

            isTooClose();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("LightProjectile"))
            {
                var positionVector = other.transform.position;
                Instantiate(damageParticleSystem, positionVector, transform.rotation);
                Destroy(other.gameObject);
                health -= damageByLightAttack;
                
                FindObjectOfType<AudioManager>().PlayOneShot(Clip.EnemyHitByLightAttack);

                if (!(health <= 0)) return;
                
                Instantiate(killParticleSystem, positionVector, transform.rotation);
                Instantiate(FindObjectOfType<Inventory>().goldPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
                
                FindObjectOfType<AudioManager>().PlayOneShot(Clip.EnemyKilled);
            }
            else if (other.transform.CompareTag("HeavyProjectile"))
            {
                var positionVector = other.transform.position;
                Instantiate(damageParticleSystem, positionVector, transform.rotation);
                Destroy(other.gameObject);
                health -= damageByHeavyAttack;

                FindObjectOfType<AudioManager>().PlayOneShot(Clip.EnemyHitByHeavyAttack);
                
                if (!(health <= 0)) return;
                
                Instantiate(killParticleSystem, positionVector, transform.rotation);
                Instantiate(FindObjectOfType<Inventory>().goldPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
                
                FindObjectOfType<AudioManager>().PlayOneShot(Clip.EnemyKilled);
            }
        }

        private void isTooClose()
        {
            var hitColliders = Physics.OverlapSphere(transform.position, re);

            foreach (var hitCollider in hitColliders)
            {
                //Add other enemy types.
                if ((hitCollider.transform.CompareTag("skelly") || hitCollider.transform.CompareTag("warchief") || hitCollider.transform.CompareTag("shaman") || hitCollider.transform.CompareTag("charger")) && hitCollider.transform != gameObject.transform)
                {
                    if((hitCollider.transform.position - transform.position).magnitude < re)
                    {
                        var direction = ((hitCollider.transform.position - this.transform.position).normalized) * re;
                        hitCollider.transform.position = transform.position + direction;
                    }
                    
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

        public void ResetAttackTimer()
        {
            attackTimer = damageCooldown;
        }

        public Animator Animator => animator;
        public PathfindingAgent Agent => agent;
        public PlayerController Player => player;
        public float AttackDistance => attackDistance;
        public float ChaseDistance => chaseDistance;
        public int Damage => damage;
        public bool HasAnimationAttack => hasAnimationAttack;
        public EnemyType Type => type;
        public EnemyType UpgradeType => upgradeType;
        public bool CanUpgrade => state.name.Equals(StateName.Formation);
        public bool CanAttack => attackTimer < 0f;
        public bool StateBlocked { set => stateBlocked = value; }
    }
}

public enum EnemyType { Skeleton, MutantCharger, GoblinShaman, GoblinWarchief }