using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackProjectilePool : MonoBehaviour
{

    private List<BlackProjectile> currentProjectiles;
    private List<BlackProjectile> backupProjectiles;

    [SerializeField]
    [Range(50, 2000)]
    private int poolSize = 50;

    [SerializeField]
    private bool spawnMore = true;

    private int projectileCount = 0;
    private Transform poolContainer;

    private BlackProjectile projectilePrefab;
    void Start()
    {
        projectilePrefab = LoadProjectilePrefab();
        if (projectilePrefab == null) {
            Debug.LogWarning("Could not find BlackProjectile prefab from Resources/Prefabs/Projectile!");
            return;
        }
        currentProjectiles = new List<BlackProjectile>();
        backupProjectiles = new List<BlackProjectile>();
        poolContainer = new GameObject("poolContainer").transform;
        poolContainer.parent = transform;
        for (int index = 0; index < poolSize; index += 1)
        {
            backupProjectiles.Add(SpawnProjectile());
        }
    }

    private BlackProjectile LoadProjectilePrefab() {
        return ((GameObject) Resources.Load("Prefabs/BlackProjectile")).GetComponent<BlackProjectile>();
    }

    private BlackProjectile SpawnProjectile()
    {
        BlackProjectile newProjectile = Instantiate(projectilePrefab);
        projectileCount += 1;
        newProjectile.name = "BlackProjectile" + projectileCount;
        newProjectile.transform.SetParent(poolContainer, true);
        return newProjectile;
    }

    public void Sleep(BlackProjectile projectile)
    {
        currentProjectiles.Remove(projectile);
        projectile.Deactivate();
        projectile.transform.SetParent(poolContainer, true);
        projectile.gameObject.SetActive(false);
        backupProjectiles.Add(projectile);
    }

    public BlackProjectile GetProjectile()
    {
        return WakeUp();
    }

    private BlackProjectile WakeUp()
    {
        if (backupProjectiles.Count <= 2)
        {
            if (spawnMore)
            {
                backupProjectiles.Add(SpawnProjectile());
            }
        }
        BlackProjectile newProjectile = null;
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
