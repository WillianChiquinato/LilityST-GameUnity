using UnityEngine;

public enum CategoriasInfo
{
    Quest,
    Comum
}

[CreateAssetMenu(fileName = "New Info Data", menuName = "Data/Info")]
public class InfoData : ScriptableObject
{
    [field: SerializeField] public string id { get; set; }

    public CategoriasInfo categoriasInfo;
    public string InfoName;
    public Sprite Icon;
    public Sprite Ilustration;
    public string Description;
    public bool obtida;

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}

