using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BlackProjectilePool))]
public class BlackProjectileManager : MonoBehaviour
{
    public static BlackProjectileManager main;

    private BlackProjectilePool pool;

    void Awake () {
        main = this;
    }

    void Start()
    {
        pool = GetComponent<BlackProjectilePool>();
        if (pool == null) {
            Debug.LogWarning("No pool was found!");
        }
    }

    public BlackProjectile SpawnProjectile(Vector3 position, Vector3 direction) {
        BlackProjectile newProjectile = pool.GetProjectile();
        newProjectile.Initialize(position, direction);
        return newProjectile;
    }

    public void Sleep(BlackProjectile projectile) {
        pool.Sleep(projectile);
    }

    void Update()
    {
        
    }
}
