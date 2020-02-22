using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        enemy = GetComponent<Enemy>();
        wand.SetOptions(GetWandOptions());
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
        }
    }

    MagicWandOptions GetWandOptions()
    {
        var opts = MagicWand.GetRandomOptions(1.0f);

        opts.damageLayerMask = LayerMask.GetMask("Player");
        opts.ProjectileLayer = LayerMask.NameToLayer("EnemyProjectile");
        opts.ProjectileTag = "EnemyProjectile";

        return opts;
    }
}
