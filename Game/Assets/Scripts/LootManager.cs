﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LootTable
{
    public float WandChance;
    public float HealthPotionChance;
    public float BootsChance;
}

public class LootManager : MonoBehaviour
{
    LootTable MeleeLootTable = new LootTable()
    {
        WandChance = 0.10f,
        HealthPotionChance = 0.25f,
        BootsChance = 0.05f
    };

    LootTable RangedLootTable = new LootTable()
    {
        WandChance = 0.20f,
        HealthPotionChance = 0.1f,
        BootsChance = 0.10f
    };

    LootTable BossLootTable = new LootTable()
    {
        WandChance = 0.00f,
        HealthPotionChance = 1.00f,
        BootsChance = 1.00f
    };


    public static LootManager main;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        
    }

    public enum EnemyType
    {
        ENEMY_MELEE,
        ENEMY_RANGED,
        BOSS
    }

    LevelDepthConfig depthConfig;

    public void SetConfig(LevelDepthConfig levelDepthConfig)
    {
        depthConfig = levelDepthConfig;
        LoadPrefabs();
    }

    public List<GameObject> GetLoot(GameObject gameObject)
    {
        EnemyType type = DetermineEnemyType(gameObject);

        var lootTable = GetLootTable(type);

        var loot = GetLoot(lootTable);

        if (type == EnemyType.BOSS)
        {
            loot.Add(gameObject.GetComponent<Boss>().wand.gameObject);
        }

        return loot;
    }

    public EnemyType DetermineEnemyType(GameObject gameObject)
    {
        var melee = gameObject.GetComponent<Meleeer>();
        if (melee != null) return EnemyType.ENEMY_MELEE;

        var ranged = gameObject.GetComponent<Ranger>();
        if (ranged != null) return EnemyType.ENEMY_RANGED;

        var boss = gameObject.GetComponent<Boss>();
        if (boss != null) return EnemyType.BOSS;

        return EnemyType.ENEMY_MELEE;
    }

    public LootTable GetLootTable(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.ENEMY_MELEE:
                return MeleeLootTable;
            case EnemyType.ENEMY_RANGED:
                return RangedLootTable;
            case EnemyType.BOSS:
                return BossLootTable;
            default:
                return MeleeLootTable;
        }
    }

    public List<GameObject> GetLoot(LootTable lootTable)
    {
        var loots = new List<GameObject>();

        var random = Random.Range(0.0f, 1.0f);
        if (random < lootTable.WandChance)
        {
            GameObject wandGameObject = Instantiate(WandPrefab);
            MagicWand wand = wandGameObject.GetComponent<MagicWand>();
            wand.SetOptions(MagicWand.GetOptions(depthConfig.PowerLevel));
            loots.Add(wandGameObject);
        }

        random = Random.Range(0.0f, 1.0f);
        if (random < lootTable.BootsChance)
        {
            GameObject bootsGameObject = Instantiate(BootsPrefab);
            Boots boots = bootsGameObject.GetComponent<Boots>();
            boots.SetOptions(Boots.GetOptions(depthConfig.PowerLevel));
            loots.Add(bootsGameObject);
        }

        random = Random.Range(0.0f, 1.0f);
        if (random < lootTable.HealthPotionChance)
        {
            GameObject potionGameObject = Instantiate(PotionPrefab);
            loots.Add(potionGameObject);
        }

        return loots;
    }


    GameObject WandPrefab;
    GameObject PotionPrefab;
    GameObject BootsPrefab;

    private void LoadPrefabs()
    {
        WandPrefab = (GameObject)Resources.Load("MagicWand");
        PotionPrefab = (GameObject)Resources.Load("Potion");
        BootsPrefab = (GameObject)Resources.Load("Boots");
    }

}
