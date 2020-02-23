using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private float lifeTime = 5f;

    private float lifeTimer = 0f;

    private Rigidbody rb;
    private Collider collider;

    MagicWandOptions options;
    float startTime;
    Vector2 direction;

    [SerializeField]
    ParticleSystem explosion;
    [SerializeField]
    GameObject proj;
    [SerializeField]
    ParticleSystem trail;

    bool flying = false;

    private void Start()
    {
        InitComponents();
        proj.SetActive(false);
        transform.position = new Vector3(10000, 10000, 10000);
    }

    public void InitComponents()
    {
        if (rb == null)
        {
            rb = GetComponentInChildren<Rigidbody>();
        }
        if (collider == null)
        {
            collider = GetComponentInChildren<Collider>();
        }
    }

    public void Explode()
    {
        flying = false;
        explosion.Play();
        trail.Stop();
        proj.SetActive(false);
        Invoke("Sleep", 1.0f);

        foreach (var collider in Physics.OverlapSphere(transform.position, options.ProjectileBlastAoE, options.damageLayerMask))
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Hurt(options.ProjectileDamage, transform.position);
            }
            Player player = collider.GetComponent<Player>();
            if (player != null)
            {
                player.Hurt(options.ProjectileDamage, transform.position);
            }
            Dummy dummy = collider.GetComponent<Dummy>();
            if (dummy != null)
            {
                dummy.GetHit(options.ProjectileDamage);
            }
        }
        /*
        Debug.DrawLine(transform.position, transform.position + Vector3.up * options.ProjectileBlastAoE, Color.white, 1.0f);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * options.ProjectileBlastAoE, Color.white, 1.0f);
        Debug.DrawLine(transform.position, transform.position + Vector3.left * options.ProjectileBlastAoE, Color.white, 1.0f);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * options.ProjectileBlastAoE, Color.white, 1.0f);
        */
    }

    public void Sleep() {
        ProjectileManager.main.Sleep(this);
    }

    public void Deactivate() {
        transform.position = new Vector3(10000, 10000, 10000);
        rb.velocity = Vector3.zero;
        lifeTimer = 0f;
    }

    public void Initialize(MagicWandOptions options, Vector2 position, Vector2 direction)
    {
        InitComponents();
        this.options = options;
        transform.position = position;
        this.lifeTime = options.ProjectileLifeTime;
        rb.velocity = direction.normalized * options.ProjectileSpeed;
        gameObject.tag = options.ProjectileTag;
        gameObject.layer = options.ProjectileLayer;
        collider.gameObject.layer = options.ProjectileLayer;

        var main = explosion.main;
        main.startColor = options.color;

        var trailGradient = trail.colorOverLifetime;
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(options.color, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) });
        trailGradient.color = grad;

        this.direction = direction;
        startTime = Time.time;
        proj.SetActive(true);
        flying = true;

        explosion.transform.localScale = new Vector3(6 * options.ProjectileBlastAoE, 6 * options.ProjectileBlastAoE, 1.0f);
        lifeTimer = 0.0f;
        trail.Play();
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;
        if (flying && lifeTimer > lifeTime)
        {
            Explode();
        }
    }

    void FixedUpdate()
    {
        if (flying)
        {
            var variance = Mathf.Sin((Time.time - startTime) * options.ProjectileVarianceFrequency);
            var dir = direction.normalized;
            var perpendicular = Vector2.Perpendicular(dir);
            rb.velocity = options.ProjectileSpeed * (dir + dir * variance * options.ProjectileVarianceY + perpendicular * variance * options.ProjectileVarianceX);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void Activate() {

    }

    void OnTriggerEnter(Collider collider)
    {
        Explode();
    }
}
