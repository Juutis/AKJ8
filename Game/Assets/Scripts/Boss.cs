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

    bool hasLineOfSight = false;
    int losLayerMask = 0;

    void Start()
    {
        InvokeRepeating("CheckLineOfSight", 0.1f, 0.1f);
        losLayerMask = LayerMask.GetMask("Player", "Wall");
    }
    
    public void CheckLineOfSight()
    {
        if (enemy.GetSimpleDistanceToPlayer() > enemy.aggroRange)
        {
            hasLineOfSight = false;
            return;
        }
        RaycastHit hit;
        var rayDirection = player.transform.position - transform.position;
        if (Physics.Raycast(transform.position + new Vector3(), rayDirection, out hit, 1000.0f, losLayerMask))
        {
            if (hit.transform.gameObject == player.gameObject)
            {
                hasLineOfSight = true;
            }
            else
            {
                hasLineOfSight = false;
            }
        }
    }

    public void Initialize(int level)
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        enemy = GetComponent<Enemy>();
        wand.SetOptions(GetWandOptions(level));
        enemy.health = 25 + 10 * level;

        maxHealth = enemy.health;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.isActiveAndEnabled)
        {
            if (hasLineOfSight &&  enemy.GetSimpleDistanceToPlayer() < attackRange)
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
        NavMeshHit hit;
        while(true)
        {
            var target = transform.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            if (NavMesh.SamplePosition(target, out hit, 0.25f, NavMesh.AllAreas))
            {
                transform.position = target;
                break;
            }
        }
    }
}
