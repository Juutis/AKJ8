using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Character))]
public class Enemy : MonoBehaviour
{
    private Character character;
    private Rigidbody rb;

    private NavMeshPath path;
    private int cornerIndex;

    private float pathingInterval = 0.2f;

    private GameObject player;

    float moveSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        path = new NavMeshPath();
        Invoke("UpdatePathing", pathingInterval);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if (path != null && path.corners.Length > 0)
        {
            var nextCorner = getNextCorner();
            var moveDir = nextCorner - (Vector2)transform.position;
            if (IsLastCorner() && distanceToNextCorner() < 1.0f)
            {
                rb.velocity = Vector3.zero;
            }
            else
            {
                rb.velocity = moveDir.normalized * moveSpeed;
            }
            character.SetTarget(nextCorner);
        }

    }

    private Vector2 getNextCorner()
    {
        while (!IsLastCorner() && distanceToNextCorner() < 0.1f)
        {
            cornerIndex++;
        }
        return path.corners[cornerIndex];
    }

    private float distanceToNextCorner()
    {
        return Vector2.Distance(path.corners[cornerIndex], transform.position);
    }

    private bool IsLastCorner()
    {
        return cornerIndex >= path.corners.Length - 1;
    }

    private void UpdatePathing()
    {
        GetPathTo(player.transform.position);
        Invoke("UpdatePathing", pathingInterval);
    }


    private void GetPathTo(Vector2 target)
    {
        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
        cornerIndex = 0;
    }
}
