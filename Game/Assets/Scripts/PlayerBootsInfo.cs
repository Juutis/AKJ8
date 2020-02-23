using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BootsInfo))]
public class PlayerBootsInfo : MonoBehaviour
{
    Player player;
    BootsInfo info;

    [SerializeField]
    GameObject container;

    // Start is called before the first frame update
    void Start()
    {
        info = GetComponent<BootsInfo>();

        InvokeRepeating("FindPlayer", 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (player.boots != null)
            {
                container.SetActive(true);
                info.UpdateUI(player.boots);
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
