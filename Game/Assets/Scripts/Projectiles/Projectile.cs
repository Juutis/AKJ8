using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{

    private float lifeTime = 5f;

    private float lifeTimer = 0f;

    private Rigidbody rb;

    SpriteRenderer rend;

    MagicWandOptions options;
    float startTime;
    Vector2 direction;

    [SerializeField]
    ParticleSystem explosion;

    private void Start()
    {
        InitComponents();
    }

    public void InitComponents()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        if (rend == null)
        {
            rend = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void Sleep() {
        explosion.Play();
        ProjectileManager.main.Sleep(this);
    }

    public void Deactivate() {
        rb.velocity = Vector3.zero;
        lifeTimer = 0f;
    }

    public void Initialize(MagicWandOptions options, Vector2 position, Vector2 direction)
    {
        InitComponents();
        transform.position = position;
        this.lifeTime = options.ProjectileLifeTime;
        rb.velocity = direction.normalized * options.ProjectileSpeed;
        //gameObject.tag = options.ProjectileTag;
        //gameObject.layer = LayerMask.NameToLayer(options.ProjectileLayer);
        rend.color = options.color;
        this.options = options;
        this.direction = direction;
        startTime = Time.time;
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer > lifeTime) {
            Sleep();
        }
    }

    private void FixedUpdate()
    {
        var variance = Mathf.Sin((Time.time - startTime) * options.ProjectileVarianceFrequency);
        var dir = direction.normalized;
        var perpendicular = Vector2.Perpendicular(dir);
        rb.velocity = options.ProjectileSpeed * (dir + dir * variance * options.ProjectileVarianceY + perpendicular * variance * options.ProjectileVarianceX);
    }

    public void Activate() {

    }

    private void OnTriggerEnter2D(Collider2D collider2d) {
        Sleep();
    }
}
