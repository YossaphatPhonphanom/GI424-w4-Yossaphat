using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class TreeInstancer : MonoBehaviour
{
    public Mesh treeMesh;
    public Material treeMaterial;
    public int count = 100;
    public float spacing = 2f;

    private List<Matrix4x4> matrices = new List<Matrix4x4>();
    private Matrix4x4[] matrixArray;

    private Vector3[] positions;
    private SphericalHarmonicsL2[] lightProbes;
    private Vector4[] occlusionProbes;

    void Start()
    {
        positions = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            float x = (i % 10) * spacing;
            float z = (i / 10) * spacing;
            Vector3 pos = new Vector3(x, 0, z);

            positions[i] = pos;
            matrices.Add(Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one));
        }

        matrixArray = matrices.ToArray();

        // หาค่า Light Probes จากตำแหน่ง
        lightProbes = new SphericalHarmonicsL2[count];
        occlusionProbes = new Vector4[count];
        LightProbes.CalculateInterpolatedLightAndOcclusionProbes(positions, lightProbes, occlusionProbes);
    }

    void Update()
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.CopySHCoefficientArraysFrom(lightProbes);
        mpb.CopyProbeOcclusionArrayFrom(occlusionProbes);

        Graphics.DrawMeshInstanced(treeMesh, 0, treeMaterial, matrixArray, count, mpb,
            UnityEngine.Rendering.ShadowCastingMode.On, true, 0, null,
            UnityEngine.Rendering.LightProbeUsage.CustomProvided, null);
    }
}