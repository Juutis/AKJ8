using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshBaker : MonoBehaviour
{
    private NavMeshSurface surface;

    public void Bake() {
        surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
        foreach(Transform child in transform) {
            Destroy(child.gameObject);
        }
    }
}
