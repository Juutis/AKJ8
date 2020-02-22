using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProjectilePool))]
public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager main;

    private ProjectilePool pool;

    void Awake () {
        main = this;
    }

    void Start()
    {
        pool = GetComponent<ProjectilePool>();
        if (pool == null) {
            Debug.LogWarning("No pool was found!");
        }
    }

    public Projectile SpawnProjectile(MagicWandOptions options, Vector3 position, Vector3 direction) {
        Projectile newProjectile = pool.GetProjectile();
        newProjectile.Initialize(options, position, direction);
        return newProjectile;
    }

    public void Sleep(Projectile projectile) {
        pool.Sleep(projectile);
    }

    void Update()
    {
        
    }
}
