using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WandInfo : MonoBehaviour
{
    MagicWand wand;

    [SerializeField]
    Text specs;

    [SerializeField]
    Image icon;

    [SerializeField]
    Image cooldownOverlay;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (wand != null)
        {
            if (wand.cooldown > 0.01f)
            {
                cooldownOverlay.fillAmount = wand.cooldown * wand.options.fireRate;
            }
            else
            {
                cooldownOverlay.fillAmount = 0.0f;
            }
        }
    }

    public void UpdateUI(MagicWand wand)
    {
        if (this.wand != wand)
        {
            this.wand = wand;
            specs.text =
                "Damage: " + wand.options.ProjectileDamage.ToString("0.0") + "\n" + 
                "Fire rate: " + wand.options.fireRate.ToString("0.0") + " per second\n" +
                "Range: " + (wand.options.ProjectileSpeed * wand.options.ProjectileLifeTime).ToString("0.0") + "\n" +

                // for debugging
                "\n" +
                "ProjectileBlastAoE: " + wand.options.ProjectileBlastAoE + "\n" +
                "ProjectileLifeTime: " + wand.options.ProjectileLifeTime + "\n" +
                "ProjectileSpeed: " + wand.options.ProjectileSpeed + "\n" +
                "projectilesPerCast: " + wand.options.projectilesPerCast + "\n" +
                "ProjectileVarianceFrequency: " + wand.options.ProjectileVarianceFrequency + "\n" +
                "ProjectileVarianceX: " + wand.options.ProjectileVarianceX + "\n" +
                "ProjectileVarianceY: " + wand.options.ProjectileVarianceY + "\n";

            icon.color = wand.options.color;
        }
    }
}
