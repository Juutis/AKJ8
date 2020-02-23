using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class FollowerCamera : MonoBehaviour
{
    private Transform target;
    private Vector3 newPosition;

    private Vector3 currentVelocity;

    private bool canFollow = false;

    [SerializeField]
    [Range(0f, 5f)]
    private float smoothing = 0.5f;

    [Tag]
    [SerializeField]
    private string targetTag = "";


    public void StartFollowing()
    {
        FindTarget();
        if (target != null) {
            canFollow = true;
        } else {
            Debug.LogWarning(string.Format("Camera couldn't find target ({0})!", targetTag));
        }
    }

    public void StopFollowing() {
        canFollow = false;
    }

    private void FindTarget() {
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
    }

    void Update()
    {
        if (!canFollow) {
            return;
        }
        newPosition = target.position;
        newPosition.z = transform.position.z;
        
        transform.position = Vector3.SmoothDamp(
            transform.position,
            newPosition,
            ref currentVelocity,
            smoothing
        );
    }

}
