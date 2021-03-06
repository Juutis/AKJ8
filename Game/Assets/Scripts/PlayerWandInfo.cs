﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WandInfo))]
public class PlayerWandInfo : MonoBehaviour
{
    Player player;
    WandInfo wandInfo;

    [SerializeField]
    GameObject container;

    // Start is called before the first frame update
    void Start()
    {
        wandInfo = GetComponent<WandInfo>();
        InvokeRepeating("FindPlayer", 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (player.magicWand != null)
            {
                container.SetActive(true);
                wandInfo.UpdateUI(player.magicWand);
            }
            else
            {
                container.SetActive(false);
            }
        }
    }

    void FindPlayer()
    {
        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null)
            {
                player = go.GetComponent<Player>();
            }
        }
    }
}
