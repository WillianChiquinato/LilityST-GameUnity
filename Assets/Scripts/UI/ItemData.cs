using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum itensType
{
    Documentos,
    Materiais
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public itensType itensType;
    public string ItemName;
    public Sprite Icon;

    [Range(0, 100)]
    public float dropChance;
}
