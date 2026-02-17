using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BowVisualMode : MonoBehaviour
{
    private bool wasBowActive = false;

    [SerializeField] private List<SpriteRenderer> backgroundRenderers = new();
    private Dictionary<SpriteRenderer, Color> originalColors = new();

    [SerializeField] private Color bowColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    [SerializeField] private TilemapRenderer[] tilemapRenderer;
    [SerializeField] private Material tilemapGrayMaterial;

    private Material[] originalTilemapMaterials;

    void Start()
    {
        tilemapRenderer = GameObject.FindObjectsOfType<TilemapRenderer>();
        GameObject[] backgrounds = GameObject.FindGameObjectsWithTag("Background");

        foreach (var obj in backgrounds)
        {
            var sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                backgroundRenderers.Add(sr);
                originalColors[sr] = sr.color;
            }
        }

        if (tilemapRenderer != null)
        {
            originalTilemapMaterials = new Material[tilemapRenderer.Length];
            for (int i = 0; i < tilemapRenderer.Length; i++)
            {
                originalTilemapMaterials[i] = tilemapRenderer[i].sharedMaterial;
            }
        }
    }

    void Update()
    {
        bool isBowActive = GameManager.instance.player.arcoEffect;

        if (isBowActive != wasBowActive)
        {
            if (isBowActive)
                ApplyGray();
            else
                RestoreOriginal();

            wasBowActive = isBowActive;
        }
    }

    void ApplyGray()
    {
        foreach (var sr in backgroundRenderers)
            sr.color = bowColor;

        if (tilemapRenderer != null)
        {
            for (int i = 0; i < tilemapRenderer.Length; i++)
            {
                tilemapRenderer[i].sharedMaterial = tilemapGrayMaterial;
            }
        }
    }

    void RestoreOriginal()
    {
        foreach (var sr in backgroundRenderers)
            sr.color = originalColors[sr];

        if (tilemapRenderer != null)
        {
            for (int i = 0; i < tilemapRenderer.Length; i++)
            {
                tilemapRenderer[i].sharedMaterial = originalTilemapMaterials[i];
            }
        }
    }
}
