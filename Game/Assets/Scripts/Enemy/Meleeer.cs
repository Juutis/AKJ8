using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meleeer : MonoBehaviour
{
    Player player;
    Enemy enemy;

    [SerializeField]
    float attackRange = 0.5f;

    [SerializeField]
    float damage = 5.0f;

    [SerializeField]
    float attackRate = 0.5f;

    [SerializeField]
    ParticleSystem attackEffect;

    bool readyToAttack = true;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.isActiveAndEnabled)
        {
            if (readyToAttack && GetSimpleDistanceToPlayer() < attackRange)
            {
                Attack();
            }

        }
    }

    private void Attack()
    {
        attackEffect.Play();
        readyToAttack = false;
        Invoke("ResetAttack", 1 / attackRate);
        player.Hurt(damage, transform.position);
    }

    private void ResetAttack()
    {
        readyToAttack = true;
    }

    private float GetSimpleDistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.transform.position);
    }
}
