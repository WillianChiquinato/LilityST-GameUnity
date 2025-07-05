using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [Header("Configs Load")]
    [SerializeField] private bool loadQuests = false;
    private int currentPlayerQuest;
    private string questSavePath => Path.Combine(Application.dataPath, "Scripts", "SaveData", "Quests");


    [Header("Quests instances")]
    private Dictionary<string, Quests> questsDictionary;
    public QuestPoint[] questPoints;
    public ScriptableObject[] QuestsInstancias;
    public List<GameObject> questGrupos;

    public GameObject instanciaQuestPrefab;
    private GameObject instanciaQuest;
    public GameObject GrupoQuest;


    [Header("UI ContainerDescrição")]
    public GameObject QuestContainer;
    public TextMeshProUGUI titleText;
    public Image IlustracaoImage;
    public TextMeshProUGUI descriptionText;
    public Button botao;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        questsDictionary = CreateQuestMap();
        questPoints = FindObjectsByType<QuestPoint>(FindObjectsSortMode.None);
        questGrupos = GameObject.FindGameObjectsWithTag("QuestContainer").OrderBy(quest => ExtractNumberFromName(quest.name)).ToList();


        // Conta quantas quests são iniciais
        int count = 0;
        foreach (Quests quest in questsDictionary.Values)
        {
            if (quest.info.StartQuest)
            {
                count++;
            }
        }

        // Agora cria o array com o tamanho correto
        QuestsInstancias = new ScriptableObject[count];

        int index = 0;
        foreach (Quests quest in questsDictionary.Values)
        {
            if (quest.info.StartQuest)
            {
                // Armazena a quest no array de quests iniciais
                QuestsInstancias[index] = quest.info;
                GameManager.instance.questEvents.StartQuest(quest.info.id);
                Debug.Log($"Quest {quest.info.NomeQuest} foi iniciada!");

                if (index < questGrupos.Count)
                {
                    GameObject questGrupo = questGrupos[index];

                    TextMeshProUGUI titleQuest = questGrupo.GetComponentInChildren<TextMeshProUGUI>();
                    Image imageQuest = questGrupo.GetComponentsInChildren<Image>().FirstOrDefault(img => img.gameObject != questGrupo);

                    if (titleQuest != null && imageQuest != null)
                    {
                        titleQuest.text = quest.info.NomeQuest;
                        imageQuest.sprite = quest.info.Icon;
                    }
                }

                index++;
            }
        }
    }

    // Extraindo apenas numeros do arrayNames
    int ExtractNumberFromName(string questName)
    {
        // Usa Regex para encontrar o primeiro número na string.
        // Usado apenas para instanciar o QuestContainer.
        Match match = Regex.Match(questName, @"\d+");

        if (match.Success && int.TryParse(match.Value, out int number))
        {
            return number;
        }

        return 0;
    }

    void OnEnable()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("Sistema_Pause não foi inicializado! Aguarde.");
            return;
        }

        GameManager.instance.questEvents.OnStartQuest += StartQuests;
        GameManager.instance.questEvents.OnAdvancedQuest += AdvancedQuests;
        GameManager.instance.questEvents.OnFinishedQuest += FinishedQuests;

        GameManager.instance.questEvents.OnQuestStepStateChanged += QuestStepStateChange;

        GameManager.instance.onPlayerChange += PlayerLevelChange;
    }

    void OnDisable()
    {
        GameManager.instance.questEvents.OnStartQuest -= StartQuests;
        GameManager.instance.questEvents.OnAdvancedQuest -= AdvancedQuests;
        GameManager.instance.questEvents.OnFinishedQuest -= FinishedQuests;

        GameManager.instance.questEvents.OnQuestStepStateChanged += QuestStepStateChange;

        GameManager.instance.onPlayerChange += PlayerLevelChange;
    }

    void Start()
    {
        foreach (Quests quest in questsDictionary.Values)
        {
            if (quest.state == QuestsState.EM_ANDAMENTO)
            {
                quest.InstantiateCurrentStep(this.transform);
            }
            GameManager.instance.questEvents.QuestStateChange(quest);
        }

        foreach (Quests quest in questsDictionary.Values)
        {
            if (quest.state == QuestsState.EM_ANDAMENTO)
            {
                // Verifica se a quest já está na UI (evita duplicar caso esteja)
                bool jaExisteNaUI = questGrupos.Any(g => g.GetComponentInChildren<TextMeshProUGUI>()?.text == quest.info.NomeQuest);

                //Refatorar isso
                if (!jaExisteNaUI)
                {
                    GameObject novaUI = Instantiate(instanciaQuestPrefab, GrupoQuest.transform);
                    questGrupos.Add(novaUI);

                    List<ScriptableObject> tempList = QuestsInstancias.ToList();
                    if (!tempList.Contains(quest.info))
                    {
                        tempList.Add(quest.info);
                        QuestsInstancias = tempList.ToArray();
                    }

                    TextMeshProUGUI titleQuest = novaUI.GetComponentInChildren<TextMeshProUGUI>();
                    if (titleQuest != null)
                    {
                        titleQuest.text = quest.info.NomeQuest;
                    }
                }

            }
        }
    }

    private void ChangeQuestState(string id, QuestsState newState)
    {
        Quests quests = GetQuestById(id);
        quests.state = newState;
        GameManager.instance.questEvents.QuestStateChange(quests);
    }

    private void PlayerLevelChange(int level)
    {
        currentPlayerQuest = level;
    }

    private bool CheckRequirementsMet(Quests quest)
    {
        bool meetsRequirements = true;

        // check player level requirements
        if (currentPlayerQuest < quest.info.levelRequisitos)
        {
            meetsRequirements = false;
        }

        foreach (QuestsInfoSO prerequisiteQuestInfo in quest.info.questsPreRequisitos)
        {
            if (GetQuestById(prerequisiteQuestInfo.id).state != QuestsState.FINALIZADO)
            {
                meetsRequirements = false;
                break;
            }
        }

        return meetsRequirements;
    }

    private void Update()
    {
        for (int i = 0; i < questGrupos.Count; i++)
        {
            GameObject obj = questGrupos[i];
            botao = obj.GetComponent<Button>();

            if (botao != null && i < QuestsInstancias.Length)
            {
                int index = i;
                QuestsInfoSO questsInfoSO = (QuestsInfoSO)QuestsInstancias[index];

                botao.onClick.RemoveAllListeners();
                botao.onClick.AddListener(() => BotaoQuestDescricoes(questsInfoSO, index));
            }
            else
            {
                Debug.LogWarning("O GameObject " + obj.name + " não tem um componente Button ou índice inválido.");
            }
        }

        for (int i = 0; i < questPoints.Length; i++)
        {
            if (GameManager.instance.playerMoviment.entrar && !questPoints[i].PlayerAtivo)
            {
                StartCoroutine(AtualizarUIQuest(questPoints[i]));
            }
        }

        foreach (Quests quest in questsDictionary.Values)
        {
            if (quest.state == QuestsState.REQUISITOS && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestsState.PODE_INICIAR);
            }
        }
    }

    IEnumerator AtualizarUIQuest(QuestPoint questPoint)
    {
        var quest = GetQuestById(questPoint.questInfopoint.id);
        if (quest == null || quest.state == QuestsState.PODE_FINALIZAR || quest.state == QuestsState.FINALIZADO)
        {
            yield break;
        }

        if (instanciaQuest == null)
        {
            instanciaQuest = Instantiate(instanciaQuestPrefab, GrupoQuest.transform);
            questGrupos.Add(instanciaQuest);

            // Adiciona o novo ScriptableObject ao array de QuestsInstancias
            List<ScriptableObject> tempList = QuestsInstancias.ToList();
            tempList.Add(questPoint.questInfopoint);
            QuestsInstancias = tempList.ToArray();

            TextMeshProUGUI titleQuest = instanciaQuest.GetComponentInChildren<TextMeshProUGUI>();
            Image imageQuest = instanciaQuest.GetComponentsInChildren<Image>().FirstOrDefault(img => img.gameObject != instanciaQuest);

            if (titleQuest != null)
            {
                titleQuest.text = questPoint.questInfopoint.NomeQuest;
            }
        }

        yield return new WaitForSeconds(1f);

        instanciaQuest = null;
    }

    public void BotaoQuestDescricoes(QuestsInfoSO questsInfoSO, int index)
    {
        if (questsInfoSO != null)
        {
            Sprite questIlustracao = questsInfoSO.IlustraçãoQuest;
            string questDescription = questsInfoSO.DescricaoQuest;

            if (titleText != null && IlustracaoImage != null)
            {
                titleText.text = questsInfoSO.NomeQuest;
                IlustracaoImage.sprite = questIlustracao;
            }
            if (descriptionText != null)
            {
                descriptionText.text = questDescription;
            }
        }
    }


    private void StartQuests(string id)
    {
        Quests quests = GetQuestById(id);

        quests.InstantiateCurrentStep(this.transform);
        ChangeQuestState(quests.info.id, QuestsState.EM_ANDAMENTO);
    }

    private void AdvancedQuests(string id)
    {
        Quests quests = GetQuestById(id);

        quests.MoveParaProximoPasso();

        if (quests.CurrentStepExists())
        {
            quests.InstantiateCurrentStep(this.transform);
        }
        else
        {
            ChangeQuestState(quests.info.id, QuestsState.PODE_FINALIZAR);
        }
    }

    private void FinishedQuests(string id)
    {
        Quests quests = GetQuestById(id);

        Debug.Log($"Finalizando quest: {id} - {quests.info.NomeQuest}");

        GameObject grupoParaRemover = questGrupos
            .FirstOrDefault(g => g.GetComponentInChildren<TextMeshProUGUI>()?.text.Trim() == quests.info.NomeQuest.Trim());

        if (grupoParaRemover == null)
        {
            Debug.LogWarning($"Nenhum grupo de UI encontrado para a quest {quests.info.NomeQuest}");
        }
        else
        {
            questGrupos.Remove(grupoParaRemover);
            Destroy(grupoParaRemover);
        }

        QuestsInstancias = QuestsInstancias.Where(q => q != quests.info).ToArray();

        ClaimRewards(quests);
        ChangeQuestState(quests.info.id, QuestsState.FINALIZADO);

        Debug.Log($"Quest {quests.info.NomeQuest} finalizada com sucesso!");
    }


    private void ClaimRewards(Quests quest)
    {
        Rewards recompensa = quest.info.recompensas;

        // XP
        if (recompensa.TipoRecompensa.HasFlag(Rewards.RewardType.XP))
        {
            if (recompensa.xpPlayer > 0)
            {
                GameManager.instance.player.PlayerXPRewards(recompensa.xpPlayer);
                Debug.Log($"Ganhou {recompensa.xpPlayer} XP!");
            }
        }

        //Itens
        if (recompensa.TipoRecompensa.HasFlag(Rewards.RewardType.Item))
        {
            if (recompensa.itensData != null)
            {
                foreach (var item in recompensa.itensData)
                {
                    inventory_System.instance.AddItem(item);
                    Debug.Log($"Ganhou carta: {item.name}");
                }
            }
        }
    }


    private void QuestStepStateChange(string id, int stepIndex, QuestsStepState questStepState)
    {
        Quests quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quests> CreateQuestMap()
    {
        QuestsInfoSO[] TodasQuests = Resources.LoadAll<QuestsInfoSO>("Quests");

        Dictionary<string, Quests> IdQuests = new Dictionary<string, Quests>();
        foreach (QuestsInfoSO quest in TodasQuests)
        {
            if (IdQuests.ContainsKey(quest.id))
            {
                Debug.LogWarning("Já existe uma quest com esse id: " + quest.id);
            }
            else
            {
                IdQuests.Add(quest.id, LoadQuest(quest));
            }
        }
        return IdQuests;
    }

    private Quests GetQuestById(string id)
    {
        Quests quests = questsDictionary[id];
        if (quests == null)
        {
            Debug.LogWarning("Não existe quest com esse id: " + id);

        }
        return quests;
    }

    public void SaveAllQuests()
    {
        foreach (Quests quest in questsDictionary.Values)
        {
            SaveQuestData(quest);
        }
    }

    private void SaveQuestData(Quests quest)
    {
        try
        {
            // Cria pasta, se não existir
            if (!Directory.Exists(questSavePath))
                Directory.CreateDirectory(questSavePath);

            QuestData questData = quest.GetQuestData();
            string serializedData = JsonUtility.ToJson(questData, true);

            string filePath = Path.Combine(questSavePath, quest.info.id + ".json");
            File.WriteAllText(filePath, serializedData);

            Debug.Log("Salvando dados da quest em: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao salvar dados da quest: " + e.Message);
        }
    }

    private Quests LoadQuest(QuestsInfoSO questsInfoSO)
    {
        Quests quest = null;
        string filePath = Path.Combine(questSavePath, questsInfoSO.id + ".json");

        try
        {
            if (File.Exists(filePath) && loadQuests)
            {
                string json = File.ReadAllText(filePath);
                QuestData questData = JsonUtility.FromJson<QuestData>(json);
                quest = new Quests(questsInfoSO, questData.state, questData.questStepIndex, questData.questStepStates);
            }
            else
            {
                quest = new Quests(questsInfoSO);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao carregar a quest: " + e.Message);
        }

        return quest;
    }
}
