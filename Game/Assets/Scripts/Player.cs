using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Character character;
    public MagicWand magicWand;
    Rigidbody rb;
    private SpriteRenderer sprite;

    Vector2 moveInput;

    float moveSpeed = 3.0f;

    int equipableMask;
    Equipable lastHovered;

    [SerializeField]
    float UseRange = 0.5f;

    [SerializeField]
    public float maxHealth = 100.0f;

    bool invincible = false, wasInvincible = false;
    float lastHurt = 0.0f;

    Color origColor;

    float health;


    [SerializeField]
    Transform keyParent;

    CursorScript cursor;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();
        EquipWand(GetComponentInChildren<MagicWand>());
        rb = GetComponent<Rigidbody>();
        equipableMask = LayerMask.GetMask("Equipable");

        foreach (var rend in GetComponentsInChildren<SpriteRenderer>())
        {
            if (rend.tag == "Character Sprite")
            {
                sprite = rend;
            }
        }
        origColor = sprite.color;

        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<CursorScript>();
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        var mousePos3d = Input.mousePosition;
        mousePos3d.z = Mathf.Abs(Camera.main.transform.position.z);
        var mousePos = Camera.main.ScreenToWorldPoint(mousePos3d);
        character.SetTarget(mousePos);

        cursor.transform.position = Input.mousePosition;

        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetButton("Shoot"))
        {
            if (magicWand != null)
            {
                magicWand.Shoot();
            }
        }


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 100, equipableMask);

        Equipable equipable = null;

        foreach (var hit in hits)
        {
            var eq = hit.collider.gameObject.GetComponent<Equipable>();
            if (eq == null || !eq.readyToPickUp)
            {
                continue;
            }
            equipable = eq;
            if (eq == lastHovered)
            {
                break;
            }
        }

        if (equipable != lastHovered)
        {
            if (lastHovered != null)
            {
                lastHovered.UnHover();
            }
            lastHovered = equipable;
        }

        if (equipable != null)
        {
            var closeEnough = Vector2.Distance(transform.position, equipable.transform.position) < UseRange;
            equipable.Hover(closeEnough);
            if (Input.GetButtonDown("Pick Up") && closeEnough && !equipable.blocked)
            {
                equipable.Equip();
                MagicWand wand = equipable.GetComponent<MagicWand>();
                if (wand != null)
                {
                    EquipWand(wand);
                }

                LevelExit exit = equipable.GetComponent<LevelExit>();
                if (exit != null)
                {
                    exit.Exit();
                }

                Key key = equipable.GetComponent<Key>();
                if (key != null)
                {
                    key.PickUp();
                    key.transform.position = keyParent.position;
                    key.transform.rotation = keyParent.rotation;
                    key.transform.parent = keyParent;
                }

                Potion potion = equipable.GetComponent<Potion>();
                if (potion != null)
                {
                    potion.PickUp();
                }
            }
            cursor.SetInteract(true);
        }
        else
        {
            cursor.SetInteract(false);
        }

        if (invincible)
        {
            var color = Color.red;
            color.a = 0.75f + 0.25f * Mathf.Cos((Time.time - lastHurt) * 5.0f);
            sprite.color = color;
        }

        if (wasInvincible && !invincible)
        {
            sprite.color = origColor;
        }
        wasInvincible = invincible;
    }

    public void EquipWand(MagicWand wand)
    {
        if (magicWand != null)
        {
            magicWand.transform.parent = transform;
            magicWand.GetComponent<Equipable>().Drop();
        }
        magicWand = wand;
        if (magicWand != null)
        {
            magicWand.options.damageLayerMask = LayerMask.GetMask("Enemy");
            magicWand.options.ProjectileLayer = LayerMask.NameToLayer("PlayerProjectile");
            magicWand.options.ProjectileTag = "PlayerProjectile";
            magicWand.transform.parent = character.sprite.transform;
            magicWand.transform.localPosition = new Vector3(0, 0.2f, 0);
            magicWand.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private void FixedUpdate()
    {
        var velocityDir = moveInput.magnitude > 1.0f ? moveInput.normalized : moveInput;
        rb.velocity = velocityDir * moveSpeed;
    }

    public void Hurt(float damage, Vector3 fromPosition)
    {
        if (!invincible)
        {
            health -= damage;
            if (health <= 0)
            {
                LevelManager.main.PlayerDie();
            }
            invincible = true;
            lastHurt = Time.time;
            Invoke("ResetInvincibility", 0.5f);
            HealthBar.main.SetHp(health);
        }
    }

    public void ResetInvincibility()
    {
        invincible = false;
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        HealthBar.main.SetHp(health);
    }
}
