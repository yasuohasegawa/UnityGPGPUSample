using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a simple gpgpu test. Try to make it simple as much as I can. 
public class Gpgpu : MonoBehaviour {
    [SerializeField]
    Mesh mesh;

    [SerializeField]
    Shader kernelShader;

    [SerializeField]
    Material material;

    [SerializeField]
    GameObject debugQuad;

    RenderTexture positionBuffer;
    RenderTexture animationBuffer;
    Material kernelMaterial;
    MaterialPropertyBlock props;

    // Use this for initialization
    void Start () {
        kernelMaterial = CreateMaterial(kernelShader);
        positionBuffer = CreateBuffer();
        animationBuffer = CreateBuffer();

        // first pass and write initial position into rendertexture.
        Graphics.Blit(null, positionBuffer, kernelMaterial, 0);

        props = new MaterialPropertyBlock();
        props.SetTexture("_PositionBuffer", positionBuffer);
        UpdatePosition();

        // second pass and write animated position.
        kernelMaterial.SetTexture("_PositionBuffer", positionBuffer);
        Graphics.Blit(null, animationBuffer, kernelMaterial, 1);

        debugQuad.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", animationBuffer);
    }

    Material CreateMaterial(Shader shader)
    {
        var material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        return material;
    }

    RenderTexture CreateBuffer()
    {
        var width = 16;
        var height = 16;
        var buffer = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        buffer.hideFlags = HideFlags.DontSave;
        buffer.filterMode = FilterMode.Point;
        buffer.wrapMode = TextureWrapMode.Repeat;
        return buffer;
    }

    private void UpdatePosition(bool isUpdate = false)
    {
        if (isUpdate) {
            kernelMaterial.SetTexture("_PositionBuffer", positionBuffer);
            Graphics.Blit(null, animationBuffer, kernelMaterial, 1);
        }

        props.SetTexture("_PositionBuffer", animationBuffer);

        var pos = transform.position;
        var rot = transform.rotation;
        var mat = material;
        var uv = new Vector2(0, 0);

        for (var y = 0; y < positionBuffer.height; y++)
        {
            for (var x = 0; x < positionBuffer.width; x++)
            {
                // normalize 0.0-1.0
                uv.x = (0.5f + x) / positionBuffer.width;
                uv.y = (0.5f + y) / positionBuffer.height;
                props.SetVector("_BufferOffset", uv);
                
                Graphics.DrawMesh(
                    mesh, pos, rot,
                    mat, 0, null, 0, props,
                    false, false
                );
                
            };
        }
    }

    // Update is called once per frame
    void Update () {
        UpdatePosition(true);
    }

    void OnDestroy()
    {
        if (positionBuffer) DestroyImmediate(positionBuffer);
        if (animationBuffer) DestroyImmediate(animationBuffer);
        if (kernelMaterial) DestroyImmediate(kernelMaterial);
    }
}
