using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{

    private float lifeTime = 5f;

    private float lifeTimer = 0f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Sleep() {
        ProjectileManager.main.Sleep(this);
    }

    public void Deactivate() {
        rb.velocity = Vector3.zero;
        lifeTimer = 0f;
    }

    public void Initialize(ProjectileOptions options) {
        transform.position = options.Position;
        this.lifeTime = options.LifeTime;
        rb.velocity = options.Direction.normalized * options.Speed;
        gameObject.tag = options.Tag;
        gameObject.layer = LayerMask.NameToLayer(options.Layer);
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer > lifeTime) {
            Sleep();
        }
    }

    public void Activate() {

    }

    private void OnTriggerEnter2D(Collider2D collider2d) {
        Sleep();
    }
}
