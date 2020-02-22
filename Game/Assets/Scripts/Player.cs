using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Character character;
    public MagicWand magicWand;
    Rigidbody rb;

    Vector2 moveInput;

    float moveSpeed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();
        EquipWand(GetComponentInChildren<MagicWand>());
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var mousePos3d = Input.mousePosition;
        mousePos3d.z = Camera.main.transform.position.z;
        var mousePos = Camera.main.ScreenToWorldPoint(mousePos3d);
        character.SetTarget(mousePos);

        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetButton("Shoot"))
        {
            magicWand.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            magicWand.SetOptions(MagicWand.GetRandomOptions(Random.Range(0.0f, 1.0f)));
            EquipWand(magicWand);
        }
    }

    public void EquipWand(MagicWand wand)
    {
        magicWand = wand;
        magicWand.options.damageLayerMask = LayerMask.GetMask("Enemy");
        magicWand.options.ProjectileLayer = LayerMask.NameToLayer("PlayerProjectile");
        magicWand.options.ProjectileTag = "PlayerProjectile";
    }

    private void FixedUpdate()
    {
        var velocityDir = moveInput.magnitude > 1.0f ? moveInput.normalized : moveInput;
        rb.velocity = velocityDir * moveSpeed;
    }
}
