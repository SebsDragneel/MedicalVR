﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("GoogleVR/UI/GvrReticlePointer")]
[RequireComponent(typeof(Renderer))]
public class _TReticle : MonoBehaviour
{
    private GvrReticlePointerImpl reticlePointerImpl;

    /// Number of segments making the reticle circle.
    public int reticleSegments = 20;

    /// Growth speed multiplier for the reticle/
    public float reticleGrowthSpeed = 8.0f;

    private void OnEnable()
    {
        _TSizeChange sc = GetComponent<_TSizeChange>();
        sc.Inititalize();
        sc.ResetToSmall();
        sc.StartGrow();
    }

    void Awake()
    {
        reticlePointerImpl = new GvrReticlePointerImpl();
    }

    void Start()
    {
        reticlePointerImpl.OnStart();
        reticlePointerImpl.MaterialComp = gameObject.GetComponent<Renderer>().material;
        UpdateReticleProperties();
        CreateReticleVertices();
    }

    void Update()
    {
        UpdateReticleProperties();
        reticlePointerImpl.UpdateDiameters();
    }

    public void SetAsMainPointer()
    {
        GvrPointerManager.Pointer = reticlePointerImpl;
    }

    private void CreateReticleVertices()
    {
        Mesh mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>();
        GetComponent<MeshFilter>().mesh = mesh;

        int segments_count = reticleSegments;
        int vertex_count = (segments_count + 1) * 2;

        #region Vertices

        Vector3[] vertices = new Vector3[vertex_count];

        const float kTwoPi = Mathf.PI * 2.0f;
        int vi = 0;
        for (int si = 0; si <= segments_count; ++si)
        {
            // Add two vertices for every circle segment: one at the beginning of the
            // prism, and one at the end of the prism.
            float angle = (float)si / (float)(segments_count) * kTwoPi;

            float x = Mathf.Sin(angle);
            float y = Mathf.Cos(angle);

            vertices[vi++] = new Vector3(x, y, 0.0f); // Outer vertex.
            vertices[vi++] = new Vector3(x, y, 1.0f); // Inner vertex.
        }
        #endregion

        #region Triangles
        int indices_count = (segments_count + 1) * 3 * 2;
        int[] indices = new int[indices_count];

        int vert = 0;
        int idx = 0;
        for (int si = 0; si < segments_count; ++si)
        {
            indices[idx++] = vert + 1;
            indices[idx++] = vert;
            indices[idx++] = vert + 2;

            indices[idx++] = vert + 1;
            indices[idx++] = vert + 2;
            indices[idx++] = vert + 3;

            vert += 2;
        }
        #endregion

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateBounds();
#if !UNITY_5_5_OR_NEWER
    // Optimize() is deprecated as of Unity 5.5.0p1.
    mesh.Optimize();
#endif  // !UNITY_5_5_OR_NEWER
    }


    private void UpdateReticleProperties()
    {
        if (reticlePointerImpl == null)
        {
            return;
        }
        reticlePointerImpl.ReticleGrowthSpeed = reticleGrowthSpeed;
        reticlePointerImpl.PointerTransform = transform;
    }
}