using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    Player player;
    Enemy enemy;

    [SerializeField]
    float attackRange = 8.0f;
    
    [SerializeField]
    public MagicWand wand;

    [SerializeField]
    ParticleSystem attackEffect;

    [SerializeField]
    ParticleSystem teleportEffect;

    float maxHealth;

    float maxTeleports = 3, teleports = 0;

    public void Initialize(int level)
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        enemy = GetComponent<Enemy>();
        wand.SetOptions(GetWandOptions(level));
        maxHealth = enemy.health;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.isActiveAndEnabled)
        {
            if (enemy.GetSimpleDistanceToPlayer() < attackRange)
            {
                wand.Shoot();
            }

            if (enemy.health < maxHealth * (maxTeleports - teleports) / (maxTeleports + 1) && teleports < maxTeleports) {
                teleports++;
                teleportEffect.Play();
                Invoke("Teleport", 0.1f);
            }
        }
    }

    MagicWandOptions GetWandOptions(int level)
    {
        var opts = MagicWand.GetOptions(level * 0.1f);

        opts.damageLayerMask = LayerMask.GetMask("Player");
        opts.ProjectileLayer = LayerMask.NameToLayer("EnemyProjectile");
        opts.ProjectileTag = "EnemyProjectile";

        return opts;
    }

    void Teleport()
    {
        NavMeshPath path = new NavMeshPath();
        while(true)
        {
            var target = transform.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            if (NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path)) 
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    transform.position = target;
                    break;
                }
            }
        }
    }
}
