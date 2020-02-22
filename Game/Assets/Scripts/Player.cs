﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Character character;
    public MagicWand magicWand;
    Rigidbody rb;

    Vector2 moveInput;

    float moveSpeed = 3.0f;

    int equipableMask;
    Equipable lastHovered;

    [SerializeField]
    float UseRange = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();
        EquipWand(GetComponentInChildren<MagicWand>());
        rb = GetComponent<Rigidbody>();
        equipableMask = LayerMask.GetMask("Equipable");
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
            if (equipable != null)
            {
                equipable.Hover();
            }
            lastHovered = equipable;
        }

        if (Input.GetButtonDown("Pick Up"))
        {
            if (equipable != null && Vector2.Distance(transform.position, equipable.transform.position) < UseRange)
            {
                equipable.Equip();
                MagicWand wand = equipable.GetComponent<MagicWand>();
                if (wand != null)
                {
                    EquipWand(wand);
                }
            }
        }
    }

    public void EquipWand(MagicWand wand)
    {
        if (magicWand != null)
        {
            magicWand.transform.localPosition = Vector3.zero;
            magicWand.transform.parent = null;
            magicWand.GetComponent<Equipable>().Drop();
            magicWand.transform.localRotation = Quaternion.Euler(Vector3.zero);
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
}
