using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProjectileOptions
{
    public Vector2 Direction;
    public Vector2 Position;

    public string Tag;
    public string Layer;

    public float LifeTime;
    public float Speed;
    public float BlastAoE;
    public float Damage;
}

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

    public Projectile SpawnProjectile(ProjectileOptions projectileOptions) {
        Projectile newProjectile = pool.GetProjectile();
        newProjectile.Initialize(projectileOptions);
        return newProjectile;
    }

    public void Sleep(Projectile projectile) {
        pool.Sleep(projectile);
    }

    void Update()
    {
        
    }
}
