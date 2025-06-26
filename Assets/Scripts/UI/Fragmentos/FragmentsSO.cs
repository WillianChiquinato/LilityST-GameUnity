using UnityEngine;

public enum fragmentoType
{
    Tempo,
    Movimento,
    Vida,
    Caos,
    Ordem
}

[CreateAssetMenu(fileName = "FragmentoInfo", menuName = "Fragment/FragmentScript", order = 1)]
public class FragmentoData : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    [Header("Geral")]
    public string NomeFragmento;
    public fragmentoType TipoFragmento;
    public Sprite Icon;
    public string DescricaoFragmento;

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
