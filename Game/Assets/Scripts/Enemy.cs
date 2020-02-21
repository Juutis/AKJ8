﻿using System.Collections;
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

    private Vector2 startPos;

    [SerializeField]
    float moveSpeed = 1.0f;

    [SerializeField]
    float aggroRange = 20.0f;

    [SerializeField]
    float desiredRange = 1.0f;

    private Routine routine;
    private Vector2 target;
    private Vector2 moveTargetPos;

    private float playerDistanceCheckTimer = 0f;

    enum Routine
    {
        PATROL,
        ATTACK
    }

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        path = new NavMeshPath();
        Invoke("UpdatePathing", pathingInterval);

        startPos = transform.position;
        target = startPos;
    }

    // Update is called once per frame
    void Update()
    {

        if (GetSimpleDistanceToPlayer() < aggroRange)
        {
            if (playerDistanceCheckTimer < Time.time)
            {
                var pathToPlayer = GetPathTo(player.transform.position);
                if (pathToPlayer.corners.Length > 0 && GetRemainingPathDistance(pathToPlayer, 0) < aggroRange)
                {
                    routine = Routine.ATTACK;
                }
                else
                {
                    routine = Routine.PATROL;
                }
                playerDistanceCheckTimer = Time.time + pathingInterval;
            }
        }
        else
        {
             routine = Routine.PATROL;
        }

        switch (routine)
        {
            case Routine.ATTACK:
                AttackRoutine();
                break;
            case Routine.PATROL:
                PatrolRoutine();
                break;
        }
    }

    void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, moveTargetPos) < 0.1f)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            var moveDir = moveTargetPos - (Vector2)transform.position;
            rb.velocity = moveDir.normalized * moveSpeed;
        }

    }

    private void PatrolRoutine()
    {
        target = startPos;
        if (HasPath())
        {
            if (IsLastCorner() && distanceToNextCorner() < desiredRange)
            {
                moveTargetPos = transform.position;
            }
            else
            {
                moveTargetPos = getNextCorner();
                character.SetTarget(moveTargetPos);
            }
        }
    }

    private void AttackRoutine()
    {
        target = player.transform.position;
        if (HasPath())
        {
            if (IsLastCorner() && distanceToNextCorner() < desiredRange)
            {
                moveTargetPos = transform.position;
                character.SetTarget(player.transform.position);
            }
            else
            {
                moveTargetPos = getNextCorner();
                character.SetTarget(moveTargetPos);
            }
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
        path = GetPathTo(target);
        cornerIndex = 0;
        Invoke("UpdatePathing", pathingInterval);
    }

    private bool HasPath()
    {
        return path.corners.Length > 0;
    }

    private NavMeshPath GetPathTo(Vector2 target)
    {
        NavMeshPath newPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, newPath);
        return newPath;
    }

    private float GetSimpleDistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.transform.position);
    }

    private float GetRemainingPathDistance(NavMeshPath navMeshPath, int currentCornerIndex)
    {
        float sum = 0;
        Vector2 prevPos = transform.position;
        for (int i = currentCornerIndex; i < navMeshPath.corners.Length; i++)
        {
            sum += Vector2.Distance(prevPos, navMeshPath.corners[i]);
            prevPos = navMeshPath.corners[i];
        }
        return sum;
    }
}
