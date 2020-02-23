using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField]
    public float healthRestored = 25.0f;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickUp()
    {
        player.Heal(healthRestored);
        Destroy(gameObject);
    }
}
