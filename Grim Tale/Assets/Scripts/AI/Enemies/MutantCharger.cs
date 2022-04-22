using AI.States;
using UnityEngine;

namespace AI.Enemies
{
    public class MutantCharger : Enemy
    {
        private bool knockedDown;
        private Vector3 knockedDownDirection;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("LightProjectile"))
            {
                var positionVector = other.transform.position;
                Instantiate(damageParticleSystem, positionVector, transform.rotation);
                Destroy(other.gameObject);
                health -= damageByLightAttack;

                FindObjectOfType<AudioManager>().PlayOneShot(Clip.EnemyHitByLightAttack);

                if (health <= 0)
                {
                    Instantiate(killParticleSystem, positionVector, transform.rotation);
                    Instantiate(FindObjectOfType<Inventory>().goldPrefab, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                    
                    FindObjectOfType<AudioManager>().PlayOneShot(Clip.EnemyKilled);
                }
            }
            else if (other.transform.CompareTag("HeavyProjectile"))
            {
                var positionVector = other.transform.position;
                Instantiate(damageParticleSystem, positionVector, transform.rotation);
                Destroy(other.gameObject);
                health -= damageByHeavyAttack;
                
                FindObjectOfType<AudioManager>().PlayOneShot(Clip.EnemyHitByHeavyAttack);
                
                if (health <= 0)
                {
                    Instantiate(killParticleSystem, positionVector, transform.rotation);
                    Instantiate(FindObjectOfType<Inventory>().goldPrefab, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                    
                    FindObjectOfType<AudioManager>().PlayOneShot(Clip.EnemyKilled);
                }
            }

            if (!other.CompareTag("Player") || state.name != StateName.Charge) return;

            if (knockedDown) return;
            
            knockedDownDirection = -((Charge) state).ChargeDirection.normalized;
            
            FindObjectOfType<AudioManager>().PlayOneShot(Clip.MutantCharge);
            
            agent.IsStopped = true;
            agent.Path = null;
            knockedDown = true;
            animator.SetTrigger("KnockedDown");
            PushBack();
            
            player.Damage(damage);
        }

        protected override void Update()
        {
            if (agent.Arrived && !knockedDown && state.name == StateName.Charge)
            {
                knockedDownDirection = -((Charge) state).ChargeDirection.normalized;

                agent.IsStopped = true;
                agent.Path = null;
                knockedDown = true;
                animator.SetTrigger("KnockedDown");
                PushBack();
            }
            
            base.Update();
        }

        public void StandUp()
        {
            agent.IsStopped = false;
            knockedDown = false;
            
            if(!stateBlocked)
                state = new Idle(this);
        }

        private void PushBack()
        {
            var rb = GetComponent<Rigidbody>();
            rb.AddForce(knockedDownDirection * 20f, ForceMode.Impulse);
        }
    }
}
