using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : MonoBehaviour
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

        if (BlackProjectileManager.main == null)
        {
            Debug.LogWarning("Your scene does not have a BlackProjectileManager!");
            return;
        }
        var position = transform.position;
        var direction = player.transform.position - transform.position;
        BlackProjectileManager.main.SpawnProjectile(position, direction, damage);

        Invoke("ResetAttack", 1 / attackRate);
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
