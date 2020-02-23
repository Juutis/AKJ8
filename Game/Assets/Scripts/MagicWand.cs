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
    public SoundType SoundType;
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

    [SerializeField]
    bool randomize = true;

    public float cooldown;

    // Start is called before the first frame update
    void Start()
    {
        if (randomize)
        {
            SetOptions(GetOptions(0.0f));
        }
        wandInfo.UpdateUI(this);
    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown < 0.01f)
        {
            cooldown = 0.0f;
        }
    }

    public void Shoot(bool isPlayer)
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
                if (isPlayer)
                {
                    SoundManager.main.PlaySound(options.SoundType);
                }
                dirOffset += 10;
            }
            readyToShoot = false;
            cooldown = 1 / options.fireRate;
            Invoke("ResetCooldown", 1 / options.fireRate);
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

    public static MagicWandOptions GetOptions(float powerLevel)
    {
        powerLevel = Mathf.Clamp(powerLevel, 0.0f, 1.0f);
        powerLevel = powerLevel / 2.0f + 0.5f;
        var weights = LootUtil.getRandomWeights(6, 0.6f * Random.Range(powerLevel, 3f * powerLevel));
        return new MagicWandOptions()
        {
            color = getRandomColor(),
            ProjectileVarianceX = GetRandomProjectileVariance(),
            ProjectileVarianceY = GetRandomProjectileVariance(),
            ProjectileVarianceFrequency = GetRandomProjectileVarianceFrequency(),

            projectilesPerCast = GetRandomProjectilesPerCast(weights[0]),
            fireRate = GetRandomFireRate(weights[1]),
            ProjectileLifeTime = GetRandomProjectileLifeTime(weights[2]),
            ProjectileSpeed = GetRandomProjectileSpeed(weights[3]),
            ProjectileBlastAoE = GetRandomProjectileBlastAoE(weights[4]),
            ProjectileDamage = GetRandomProjectileDamage(weights[5]),
            SoundType = GetRandomWandSoundType()
        };
    }

    private static List<SoundType> randomWandsounds = new List<SoundType>() {
        SoundType.Wand1,
        SoundType.Wand2,
        SoundType.Wand3,
        SoundType.Wand4,
        SoundType.Wand5,
        SoundType.Wand6,
        SoundType.Wand7,
        SoundType.Wand8
    };

    private static SoundType GetRandomWandSoundType()
    {
        return randomWandsounds[Random.Range(0, randomWandsounds.Count)];
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
        //return Mathf.Clamp((int)Random.Range(-3, 3 + 10*level), 1, 5);
        return Mathf.Clamp((int)(-3 + 10 * level), 1, 5);
    }

    private static float GetRandomFireRate(float level)
    {
        //return Random.Range(0.5f, 5 + level * 5);
        return 0.5f + 5f * level;
    }

    private static float GetRandomProjectileLifeTime(float level)
    {
        //return Random.Range(0.5f, 2 + level * 5);
        return 1.0f + level * 6f;
    }

    private static float GetRandomProjectileSpeed(float level)
    {
        //return Random.Range(3.0f, 5 + level * 5);
        return 3.0f + level * 7f;
    }

    private static float GetRandomProjectileBlastAoE(float level)
    {
        //return Random.Range(0.1f, 0.1f + level * 1.5f);
        return 0.1f + level * 1.4f;
    }

    private static float GetRandomProjectileDamage(float level)
    {
        //return Random.Range(1.0f, 3.0f + level * 12.0f);
        return 1.0f + level * 14.0f;
    }

    private static float GetRandomProjectileVariance()
    {
        return Random.Range(-1.0f, 1.0f);
    }

    private static float GetRandomProjectileVarianceFrequency()
    {
        var value = Random.Range(-10.0f, 10.0f);
        if (value < 5.0f && value > -5.0f) value = 0.0f;
        return value;
    }
}
