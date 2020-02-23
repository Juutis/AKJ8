using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WandInfo))]
public class PlayerWandInfo : MonoBehaviour
{
    Player player;
    WandInfo wandInfo;

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        wandInfo = GetComponent<WandInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && player.magicWand != null)
        {
            wandInfo.UpdateUI(player.magicWand.options);
        }
    }
}
