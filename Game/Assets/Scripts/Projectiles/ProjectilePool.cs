using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{

    private List<Projectile> currentProjectiles;
    private List<Projectile> backupProjectiles;

    [SerializeField]
    [Range(50, 200)]
    private int poolSize = 50;

    [SerializeField]
    private bool spawnMore = true;

    private int projectileCount = 0;
    private Transform poolContainer;

    private Projectile projectilePrefab;
    void Start()
    {
        projectilePrefab = LoadProjectilePrefab();
        if (projectilePrefab == null) {
            Debug.LogWarning("Could not find Projectile prefab from Resources/Prefabs/Projectile!");
            return;
        }
        currentProjectiles = new List<Projectile>();
        backupProjectiles = new List<Projectile>();
        poolContainer = new GameObject("poolContainer").transform;
        poolContainer.parent = transform;
        for (int index = 0; index < poolSize; index += 1)
        {
            backupProjectiles.Add(SpawnProjectile());
        }
    }

    private Projectile LoadProjectilePrefab() {
        return ((GameObject) Resources.Load("Prefabs/Projectile")).GetComponent<Projectile>();
    }

    private Projectile SpawnProjectile()
    {
        Projectile newProjectile = Instantiate(projectilePrefab);
        projectileCount += 1;
        newProjectile.name = "Projectile" + projectileCount;
        newProjectile.transform.SetParent(poolContainer, true);
        return newProjectile;
    }

    public void Sleep(Projectile projectile)
    {
        currentProjectiles.Remove(projectile);
        projectile.Deactivate();
        projectile.transform.SetParent(poolContainer, true);
        projectile.gameObject.SetActive(false);
        backupProjectiles.Add(projectile);
    }

    public Projectile GetProjectile()
    {
        return WakeUp();
    }

    private Projectile WakeUp()
    {
        if (backupProjectiles.Count <= 2)
        {
            if (spawnMore)
            {
                backupProjectiles.Add(SpawnProjectile());
            }
        }
        Projectile newProjectile = null;
        if (backupProjectiles.Count > 0)
        {
            newProjectile = backupProjectiles[0];
            backupProjectiles.RemoveAt(0);
            newProjectile.gameObject.SetActive(true);
            newProjectile.Activate();
            currentProjectiles.Add(newProjectile);
        } else {
            Debug.LogWarning("Projectile pool limit reached!");
        }
        return newProjectile;
    }

}
