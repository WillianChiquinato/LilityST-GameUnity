using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "QuestsInfo", menuName = "Quests/QuestsScript", order = 1)]
public class QuestsInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    [Header("Geral")]
    public string NomeQuest;
    public bool StartQuest = false;
    public Sprite Icon;
    public Sprite IlustraçãoQuest;

    [Header("Requisitos")]
    public int levelRequisitos;
    public QuestsInfoSO[] questsPreRequisitos;
    public string DescricaoQuest;

    [Header("Etapas")]
    public GameObject[] questsEtapasPrefabs;

    [Header("Recompensas")]
    public int xpRecompensa;

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
