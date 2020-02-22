using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour {

    private List<MeshFilter> meshFilters;
    private CombineInstance[] combineInstances;

    private MeshFilter meshFilter;

    private MeshCollider meshCollider;

    public void Combine() {
        meshFilters = new List<MeshFilter>();
        foreach(Transform child in transform) {
            MeshFilter childMeshFilter = child.GetComponent<MeshFilter>();
            if (childMeshFilter != null) {
                meshFilters.Add(childMeshFilter);
            }
        }
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        Build();
        foreach(Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    public void Build() {
        combineInstances = new CombineInstance[meshFilters.Count];
        for (int index = 0; index < meshFilters.Count; index += 1 ) {
            combineInstances[index].mesh = meshFilters[index].sharedMesh;
            combineInstances[index].transform = meshFilters[index].transform.localToWorldMatrix;
            meshFilters[index].gameObject.SetActive(false);
        }
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combineInstances);
        if (meshCollider != null) {
            meshCollider.sharedMesh = meshFilter.mesh;
        }
        name = string.Format("{0} ({1} meshes)", name, meshFilters.Count);
    }

    public void Add(MeshFilter filter) {
        meshFilters.Add(filter);
    }
}
