using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Character character;
    Shooter shooter;
    Rigidbody rb;

    Vector2 moveInput;

    float moveSpeed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();
        shooter = GetComponent<Shooter>();
        rb = GetComponent<Rigidbody>();

        shooter.SetProjectileConfig(new ProjectileOptions()
        {
            LifeTime = 5f,
            Speed = 20f,
            BlastAoE = 5f,
            Damage = 15f,
            Tag = "PlayerProjectile",
            Layer = "PlayerProjectile"
        });
    }

    // Update is called once per frame
    void Update()
    {
        var mousePos3d = Input.mousePosition;
        mousePos3d.z = Camera.main.transform.position.z;
        var mousePos = Camera.main.ScreenToWorldPoint(mousePos3d);
        character.SetTarget(mousePos);

        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetButtonDown("Shoot"))
        {
            shooter.Shoot(mousePos);
        }
    }

    private void FixedUpdate()
    {
        var velocityDir = moveInput.magnitude > 1.0f ? moveInput.normalized : moveInput;
        rb.velocity = velocityDir * moveSpeed;
    }
}
