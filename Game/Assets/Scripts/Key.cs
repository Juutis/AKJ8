using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    LevelExit exit;

    // Start is called before the first frame update
    void Start()
    {
        exit = GameObject.FindGameObjectWithTag("LevelExit").GetComponent<LevelExit>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickUp()
    {
        exit.Unlock();
    }
}
