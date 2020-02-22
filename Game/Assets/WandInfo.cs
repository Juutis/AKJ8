using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WandInfo : MonoBehaviour
{
    Player player;
    MagicWandOptions wandOptions;

    [SerializeField]
    Text wandSpecs;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        wandOptions = player.magicWand.options;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!wandOptions.Equals(player.magicWand.options))
        {
            wandOptions = player.magicWand.options;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        wandSpecs.text = "Fire rate: " + wandOptions.fireRate + "\n" +
            "Damage: " + wandOptions.ProjectileDamage + "\n" +
            "Range: " + wandOptions.ProjectileSpeed * wandOptions.ProjectileLifeTime + "\n" +

            // for debugging
            "\n" +
            "ProjectileBlastAoE: " + wandOptions.ProjectileBlastAoE + "\n" +
            "ProjectileLifeTime: " + wandOptions.ProjectileLifeTime + "\n" +
            "ProjectileSpeed: " + wandOptions.ProjectileSpeed + "\n" +
            "projectilesPerCast: " + wandOptions.projectilesPerCast + "\n" +
            "ProjectileVarianceFrequency: " + wandOptions.ProjectileVarianceFrequency + "\n" +
            "ProjectileVarianceX: " + wandOptions.ProjectileVarianceX + "\n" +
            "ProjectileVarianceY: " + wandOptions.ProjectileVarianceY + "\n";
    }
}
