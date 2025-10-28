using UnityEngine;

public enum CategoriasInfo
{
    Quest,
    Comum
}

[CreateAssetMenu(fileName = "New Info Data", menuName = "Data/Info")]
public class InfoData : ScriptableObject
{
    public string id;
    public CategoriasInfo categoriasInfo;
    public string InfoName;
    public Sprite Icon;
    public Sprite Ilustration;
    public string Description;

    public bool obtida;
}

