using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    GameObject sprite;
    Vector2 target;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var rend in GetComponentsInChildren<SpriteRenderer>())
        {
            if (rend.tag == "Character Sprite")
            {
                sprite = rend.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        var targetDir = target - MyPos();
        var angleDiff = Vector2.SignedAngle(sprite.transform.up, targetDir);
        sprite.transform.Rotate(Vector3.forward, angleDiff);
    }

    public void SetTarget(Vector2 newTarget)
    {
        target = newTarget;
    }

    private Vector2 MyPos()
    {
        return transform.position;
    }
}
