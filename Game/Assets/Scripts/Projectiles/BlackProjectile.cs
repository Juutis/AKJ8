using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackProjectile : MonoBehaviour
{

    private float lifeTime = 5f;

    private float lifeTimer = 0f;

    private Rigidbody rb;
    private Collider collider;
    
    float startTime;
    Vector2 direction;

    [SerializeField]
    ParticleSystem explosion;
    [SerializeField]
    GameObject proj;
    [SerializeField]
    ParticleSystem trail;

    bool flying = false;
    
    float damage = 5.0f;

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

        /*
        Debug.DrawLine(transform.position, transform.position + Vector3.up * options.ProjectileBlastAoE, Color.white, 1.0f);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * options.ProjectileBlastAoE, Color.white, 1.0f);
        Debug.DrawLine(transform.position, transform.position + Vector3.left * options.ProjectileBlastAoE, Color.white, 1.0f);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * options.ProjectileBlastAoE, Color.white, 1.0f);
        */
    }

    public void Sleep() {
        BlackProjectileManager.main.Sleep(this);
    }

    public void Deactivate() {
        transform.position = new Vector3(10000, 10000, 10000);
        rb.velocity = Vector3.zero;
        lifeTimer = 0f;
    }

    public void Initialize(Vector2 position, Vector2 direction, float damage)
    {
        InitComponents();
        this.damage = damage;
        transform.position = position;
        this.lifeTime = 5.0f;
        rb.velocity = direction.normalized * 4.0f;
        
        this.direction = direction;
        startTime = Time.time;
        proj.SetActive(true);
        flying = true;

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
        var player = collider.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.Hurt(damage, transform.position);
        }

        Explode();
    }
}
