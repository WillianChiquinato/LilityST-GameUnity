using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum itensType
{
    Documentos,
    Materiais,
    Coletaveis
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public itensType itensType;
    public string ItemName;
    public Sprite Icon;
    public Sprite ilustracao;
    public string Description;

    [Range(0, 100)]
    public float dropChance;
}
