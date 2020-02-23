﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    int health = 10;
    bool dead = false;
    public void GetHit(float damage){
        health -= (int)damage;
        Debug.Log(string.Format("Take {0} damage! Health: {1}", damage, health));
        if (health <= 0) {
            health = 0;
            Die();
        }
    }
    private void Die() {
        if (!dead) {
            dead = true;
            LevelManager.main.SpawnKeyAt(transform.position);
            Destroy(gameObject);
        }
    }
}
