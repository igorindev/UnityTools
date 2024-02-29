using UnityEngine;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

[Tooltip("Does not work on static meshes, make sure all meshes you want included are not flagged as 'batched' in their static settings.")]
public class MeshMerger : MonoBehaviour
{
    public enum AlgorithmType
    {
        Disable = -1,
        Unity = 0,
        OldSchool = 1
    }

    private class ObjectInstanceIDEqualityComparer<T> : EqualityComparer<T> where T : Object
    {
        public override bool Equals(T x, T y)
        {
            return x.GetInstanceID() == y.GetInstanceID();
        }

        public override int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

    private class Group
    {
        public Material material;
        public HashSet<MeshFilter> filters = new HashSet<MeshFilter>();

        public int vertCount = 0;
        public int normCount = 0;
        public int triCount = 0;
        public int uvCount = 0;
    }

    public AlgorithmType Algorithm;

    private void Start()
    {
        switch (Algorithm)
        {
            case AlgorithmType.Unity:
                MergeUnityAPI();
                break;
            case AlgorithmType.OldSchool:
                MergeUsingOldSchool();
                break;
        }
    }

    private void MergeUnityAPI()
    {
        Dictionary<Material, HashSet<MeshFilter>> table = new Dictionary<Material, HashSet<MeshFilter>>(ObjectInstanceIDEqualityComparer<Material>.Default);
        foreach (var f in this.GetComponentsInChildren<MeshFilter>())
        {
            //if (f.HasTag(Constants.TAG_IGNOREMESHMERGER)) continue;

            Mesh mesh = f.sharedMesh;
            if (mesh.GetInstanceID() < 0) continue; //this means it's a combined mesh

            if (!f.TryGetComponent<Renderer>(out var renderer)) continue;

            var mat = renderer.sharedMaterial;
            if (!table.TryGetValue(mat, out HashSet<MeshFilter> grp))
            {
                grp = new HashSet<MeshFilter>();
                table.Add(mat, grp);
            }

            grp.Add(f);
        }

        foreach (var pair in table)
        {
            var mat = pair.Key;
            var grp = pair.Value;
            var combine = new CombineInstance[grp.Count];
            var worldToLocal = this.transform.worldToLocalMatrix;

            int i = 0;
            foreach (var f in grp)
            {
                combine[i].mesh = f.sharedMesh;
                combine[i].transform = worldToLocal * f.transform.localToWorldMatrix;
                f.gameObject.SetActive(false);
                i++;
            }

            // hook up the mesh renderer
            var go = new GameObject("Mesh*" + mat.name);
            go.transform.parent = this.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            var me = new Mesh();
            go.AddComponent<MeshFilter>().sharedMesh = me;
            me.CombineMeshes(combine, true, true);
            go.AddComponent<MeshRenderer>().sharedMaterial = mat;
        }

        //cleanup
        table.Clear();
        table = null;
        GC.Collect();
    }

    private void MergeUsingOldSchool()
    {
        var table = new Dictionary<Material, Group>(ObjectInstanceIDEqualityComparer<Material>.Default);
        foreach (var f in this.GetComponentsInChildren<MeshFilter>())
        {
            var mesh = f.sharedMesh;
            if (mesh.GetInstanceID() < 0) continue; //this means it's a combined mesh

            var renderer = f.GetComponent<Renderer>();
            if (renderer == null) continue;


            var mat = renderer.sharedMaterial;
            if (!table.TryGetValue(mat, out Group grp))
            {
                grp = new Group();
                grp.material = mat;
                table.Add(mat, grp);
            }

            grp.filters.Add(f);

            grp.vertCount += mesh.vertices.Length;
            grp.normCount += mesh.normals.Length;
            grp.triCount += mesh.triangles.Length;
            grp.uvCount += mesh.uv.Length;
        }

        foreach (var grp in table.Values)
        {
            // allocate arrays
            Vector3[] verts = new Vector3[grp.vertCount];
            Vector3[] norms = new Vector3[grp.normCount];
            Matrix4x4[] bindPoses = new Matrix4x4[grp.filters.Count];
            BoneWeight[] weights = new BoneWeight[grp.vertCount];
            int[] tris = new int[grp.triCount];
            Vector2[] uvs = new Vector2[grp.uvCount];

            int vertOffset = 0;
            int normOffset = 0;
            int triOffset = 0;
            int uvOffset = 0;
            int meshOffset = 0;
            var worldToLocal = this.transform.worldToLocalMatrix;

            // merge the meshes and set up bones
            foreach (MeshFilter mf in grp.filters)
            {
                var mesh = mf.sharedMesh;

                foreach (int i in mesh.triangles)
                    tris[triOffset++] = i + vertOffset;

                bindPoses[meshOffset] = Matrix4x4.identity;

                var matrix = worldToLocal * mf.transform.localToWorldMatrix;

                foreach (Vector3 v in mesh.vertices)
                {
                    weights[vertOffset].weight0 = 1.0f;
                    weights[vertOffset].boneIndex0 = meshOffset;
                    verts[vertOffset++] = matrix.MultiplyPoint(v);
                }

                foreach (Vector3 n in mesh.normals)
                    norms[normOffset++] = matrix.MultiplyVector(n);

                foreach (Vector2 uv in mesh.uv)
                    uvs[uvOffset++] = uv;

                meshOffset++;

                MeshRenderer mr = mf.GetComponent<MeshRenderer>();
                if (mr != null)
                    mr.enabled = false;
            }

            // hook up the mesh
            Mesh me = new Mesh();
            me.name = gameObject.name;
            me.vertices = verts;
            me.normals = norms;
            me.boneWeights = weights;
            me.uv = uvs;
            me.triangles = tris;
            me.bindposes = bindPoses;

            // hook up the mesh renderer
            var go = new GameObject("Mesh*" + grp.material.name);
            go.transform.parent = this.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            go.AddComponent<MeshFilter>().sharedMesh = me;
            go.AddComponent<MeshRenderer>().sharedMaterial = grp.material;
        }

        //cleanup
        table.Clear();
        table = null;
        GC.Collect();
    }
}
