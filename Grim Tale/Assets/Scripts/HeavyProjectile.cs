using UnityEngine;

public class HeavyProjectile : MonoBehaviour
{
    [SerializeField] private float timeToDeath = 2f;

    private float speed;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward.normalized * speed * Time.deltaTime;

        if (timeToDeath > 0)
        {
            timeToDeath -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.CompareTag("Obstacle"))
        {
            FindObjectOfType<AudioManager>().PlayOneShot(Clip.ProjectileDestroy);
            Destroy(gameObject);
        }
    }
}
