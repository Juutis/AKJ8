using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    ProjectileOptions projectileOptions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Vector2 target)
    {
        if (ProjectileManager.main == null)
        {
            Debug.LogWarning("Your scene does not have a ProjectileManager!");
        }
        projectileOptions.Position = transform.position;
        projectileOptions.Direction = target - (Vector2)transform.position;
        ProjectileManager.main.SpawnProjectile(projectileOptions);
    }

    public void SetProjectileConfig(ProjectileOptions projectileOptions)
    {
        this.projectileOptions = projectileOptions;
    }
}
