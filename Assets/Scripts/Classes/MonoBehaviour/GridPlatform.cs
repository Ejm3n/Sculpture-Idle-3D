using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlatform : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material defaultMaterial;

    public void Select()
    {
        meshRenderer.material = GameData.Default.platformSelectedMatrial;
    }
    public void Deselect()
    {
        meshRenderer.material = defaultMaterial;
    }
}
