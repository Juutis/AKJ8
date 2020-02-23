using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BootsInfo : MonoBehaviour
{
    Boots boots;

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
        if (boots != null)
        {
            if (boots.cooldown > 0.01f)
            {
                cooldownOverlay.fillAmount = boots.cooldown / boots.options.teleportCooldown;
            }
            else
            {
                cooldownOverlay.fillAmount = 0.0f;
            }
        }
    }

    public void UpdateUI(Boots boots)
    {
        if (this.boots != boots)
        {
            this.boots = boots;
            string text = "Movent speed modifier: " + (boots.options.bonusSpeed >= 0 ? "+" : "") + (boots.options.bonusSpeed*100).ToString("0.0") + "%\n";
            if (boots.options.hasTeleport)
            {
                text += "Teleport distance: " + boots.options.teleportDistance.ToString("0.0") + "\n" +
                    "Teleport cooldown: " + boots.options.teleportCooldown.ToString("0.0") + "s";
            }

            specs.text = text;
            icon.color = boots.options.color;
        }
    }
}
