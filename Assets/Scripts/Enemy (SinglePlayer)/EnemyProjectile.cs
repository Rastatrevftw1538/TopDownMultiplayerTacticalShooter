using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Stats")]
    public float speed;
    public float damage;
    public float projectileLifeTime;

    public GameObject particles;

    private float lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        //particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.fixedDeltaTime);
        lifeTime += Time.deltaTime;

        if (lifeTime >= projectileLifeTime)
            DestroyProjectile();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerHealthSinglePlayer playerHealth = other.gameObject.GetComponent<PlayerHealthSinglePlayer>();
            playerHealth.TakeDamage(damage);
        }

        if (other.gameObject.tag == "Wall")
        {
            DestroyProjectile();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Wall")
        {
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
        PlayParticles();
    }

    private void PlayParticles()
    {
        GameObject effect = Instantiate(particles, transform.position, Quaternion.identity, this.gameObject.transform);
        Destroy(effect, 0.5f);
    }
}
