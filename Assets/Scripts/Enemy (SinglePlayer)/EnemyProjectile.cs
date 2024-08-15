using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Stats")]
    public float speed;
    public float damage;
    public float projectileLifeTime;
    private TrailRenderer trailRenderer;
    public List<AudioClip> shootSounds;
    private Rigidbody2D rb;

    public GameObject particles;

    private float lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        int rand = Random.Range(0, shootSounds.Count);
        if (SoundFXManager.Instance) SoundFXManager.Instance.PlaySoundFXClip(shootSounds[rand], transform, 0.05f);
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = true;
        rb = GetComponent<Rigidbody2D>();
        //particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(speed * Vector2.up * Time.fixedDeltaTime * Time.timeScale);
        UnityEngine.Vector3 moveVector = speed * Time.fixedDeltaTime * Time.timeScale * transform.TransformDirection(Vector2.up);
        rb.velocity = new UnityEngine.Vector2(moveVector.x, moveVector.y);
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
            DestroyProjectile();
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
        //PlayParticles();
        Destroy(gameObject);
    }

    private void PlayParticles()
    {
        GameObject effect = Instantiate(particles, transform.position, Quaternion.identity, this.gameObject.transform);
        Destroy(effect, 0.5f);
    }
}
