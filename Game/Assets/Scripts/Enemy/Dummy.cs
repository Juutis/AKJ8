using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    int health = 3;
    bool dead = false;
    public void GetHit(float damage){
        health -= (int)damage;
        if (health <= 0) {
            health = 0;
            Die();
        }
        var color = sprite.color;
        color.a = 0.1f;
        sprite.color = color;
        lastHurt = Time.time;
    }
    private void Die() {
        if (!dead) {
            dead = true;
            LevelManager.main.SpawnKeyAt(transform.position);
            Destroy(gameObject);
        }
    }

    float lastHurt = 0.0f;

    [SerializeField]
    private SpriteRenderer sprite;

    public void Update()
    {
        if (lastHurt > Time.time - 0.5)
        {
            var color = sprite.color;
            color.a = Mathf.Lerp(0.1f, 1.0f, (Time.time - lastHurt) / 0.5f);
            sprite.color = color;
        }
        else
        {
            var color = sprite.color;
            color.a = 1.0f;
            sprite.color = color;
        }
    }
}
