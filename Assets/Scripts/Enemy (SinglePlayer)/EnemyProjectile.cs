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
        //int rand = Random.Range(0, shootSounds.Count);
        //if (SoundFXManager.Instance) SoundFXManager.Instance.PlaySoundFXClip(shootSounds[rand], transform, 0.05f);
        trailRenderer = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        //particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    UnityEngine.Vector3 moveVector;
    void Update()
    {
        //transform.Translate(speed * Vector2.up * Time.fixedDeltaTime * Time.timeScale);
        moveVector = speed * Time.fixedDeltaTime * Time.timeScale * transform.TransformDirection(Vector2.up);
        rb.velocity = new UnityEngine.Vector2(moveVector.x, moveVector.y);
        lifeTime += Time.deltaTime;

        if (lifeTime >= projectileLifeTime)
            ResetData();
            //DestroyProjectile();
    }

    PlayerHealthSinglePlayer playerHealth;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(!playerHealth) other.gameObject.TryGetComponent<PlayerHealthSinglePlayer>(out playerHealth);
            playerHealth.TakeDamage(damage);
            //DestroyProjectile();
            ResetData();
        }

        if (other.gameObject.tag == "Wall")
        {
            //DestroyProjectile();
            ResetData();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Wall")
        {
            //DestroyProjectile();
            ResetData();
        }
    }

    private void DestroyProjectile()
    {
        //PlayParticles();
        Destroy(gameObject);
    }

    private void ResetData()
    {
        //Debug.Log("Resetting Data");
        //moveVector = Vector3.zero;
        rb.velocity = Vector2.zero;
        lifeTime = 0f;
        gameObject.SetActive(false);
    }

    private void PlayParticles()
    {
        GameObject effect = Instantiate(particles, transform.position, Quaternion.identity, this.gameObject.transform);
        Destroy(effect, 0.5f);
    }
}
