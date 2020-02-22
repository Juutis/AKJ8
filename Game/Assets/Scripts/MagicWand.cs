using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MagicWandOptions
{
    public Color color;
    public int projectilesPerCast;
    public float fireRate;

    public string ProjectileTag;
    public int ProjectileLayer;
    public int damageLayerMask;

    public float ProjectileLifeTime;
    public float ProjectileSpeed;
    public float ProjectileBlastAoE;
    public float ProjectileDamage;
    public float ProjectileVarianceX;
    public float ProjectileVarianceY;
    public float ProjectileVarianceFrequency;
}

public class MagicWand : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Transform projectileOrigin;

    [SerializeField]
    WandInfo wandInfo;
    
    public MagicWandOptions options;

    private bool readyToShoot = true;

    // Start is called before the first frame update
    void Start()
    {
        SetOptions(GetRandomOptions(0.0f));
        wandInfo.UpdateUI(options);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        if (readyToShoot)
        {
            readyToShoot = false;
            Invoke("ResetCooldown", 1 / options.fireRate);
            if (ProjectileManager.main == null)
            {
                Debug.LogWarning("Your scene does not have a ProjectileManager!");
                return;
            }
            var position = transform.position;
            var dirOffset = -(options.projectilesPerCast - 1) * 5;
            for (int i = 0; i < options.projectilesPerCast; i++)
            {
                var direction = Quaternion.Euler(0, 0, dirOffset) * transform.up;
                ProjectileManager.main.SpawnProjectile(options, position, direction);
                dirOffset += 10;
            }
            readyToShoot = false;
            Invoke("ResetCooldown", 1/options.fireRate);
        }
    }

    public void ResetCooldown()
    {
        readyToShoot = true;
    }

    public void SetOptions(MagicWandOptions options)
    {
        this.options = options;
        OptionsUpdated();
    }

    private void OptionsUpdated()
    {
        spriteRenderer.color = options.color;
    }

    public static MagicWandOptions GetRandomOptions(float level)
    {
        level = Mathf.Clamp(level, 0.0f, 1.0f);
        return new MagicWandOptions()
        {
            color = getRandomColor(),
            ProjectileVarianceX = GetRandomProjectileVariance(),
            ProjectileVarianceY = GetRandomProjectileVariance(),
            ProjectileVarianceFrequency = GetRandomProjectileVarianceFrequency(),

            projectilesPerCast = GetRandomProjectilesPerCast(level),
            fireRate = GetRandomFireRate(level),
            ProjectileLifeTime = GetRandomProjectileLifeTime(level),
            ProjectileSpeed = GetRandomProjectileSpeed(level),
            ProjectileBlastAoE = GetRandomProjectileBlastAoE(level),
            ProjectileDamage = GetRandomProjectileDamage(level)
        };
    }

    private static Color getRandomColor()
    {
        return Random.ColorHSV(
            0.0f, 1.0f, // hue
            0.0f, 1.0f, // saturation
            0.8f, 1.0f, // value
            1.0f, 1.0f); //alpha
    }

    private static int GetRandomProjectilesPerCast(float level)
    {
        return Mathf.Clamp((int)Random.Range(-3, 3 + 10*level), 1, 5);
    }

    private static float GetRandomFireRate(float level)
    {
        return Random.Range(0.5f, 5 + level * 5);
    }

    private static float GetRandomProjectileLifeTime(float level)
    {
        return Random.Range(0.5f, 2 + level * 5);
    }

    private static float GetRandomProjectileSpeed(float level)
    {
        return Random.Range(3.0f, 5 + level * 5);
    }

    private static float GetRandomProjectileBlastAoE(float level)
    {
        return Random.Range(0.1f, 0.1f + level * 1.5f);
    }

    private static float GetRandomProjectileDamage(float level)
    {
        return Random.Range(1.0f, 3.0f + level * 12.0f);
    }

    private static float GetRandomProjectileVariance()
    {
        return Random.Range(-1.0f, 1.0f);
    }

    private static float GetRandomProjectileVarianceFrequency()
    {
        return Random.Range(-10.0f, 10.0f);
    }
}
