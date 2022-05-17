using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialFadeController
{
    public Material mat;

    [Range(0f, 1f)]
    public float colorAlpha = 1.0f;

    public void InitializeColorAlpha()
    {
        colorAlpha = 1.0f;
    }

    public void SetMaterialAlpha()
    {
        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, colorAlpha);
    }
}
