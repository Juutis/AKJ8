using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    LevelExit exit;
    Collider coll;

    // Start is called before the first frame update
    void Start()
    {
        exit = GameObject.FindGameObjectWithTag("LevelExit").GetComponent<LevelExit>();
        coll = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickUp()
    {
        SoundManager.main.PlaySound(SoundType.PickUpKey);
        coll.enabled = false;
        exit.Unlock(this);
    }
}
