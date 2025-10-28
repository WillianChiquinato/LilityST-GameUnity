using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
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
        LoadAllQuests();

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

        int currentIndex = quests.CurrentStepIndex;
        quests.StoreQuestStepState(new QuestsStepState(QuestStepStates.INICIADO), currentIndex);

        quests.InstantiateCurrentStep(this.transform);
        ChangeQuestState(quests.info.id, QuestsState.EM_ANDAMENTO);
        SaveAllQuests();
    }

    private void AdvancedQuests(string id)
    {
        Quests quests = GetQuestById(id);

        // Avança para a próxima etapa
        quests.MoveParaProximoPasso();
        int currentIndex = quests.CurrentStepIndex;
        quests.StoreQuestStepState(new QuestsStepState(QuestStepStates.EM_ANDAMENTO), currentIndex);

        if (quests.CurrentStepExists())
        {
            // Marca a nova etapa como iniciada
            quests.StoreQuestStepState(new QuestsStepState(QuestStepStates.INICIADO), currentIndex);
            quests.InstantiateCurrentStep(this.transform);
        }
        else
        {
            ChangeQuestState(quests.info.id, QuestsState.PODE_FINALIZAR);
            quests.StoreQuestStepState(new QuestsStepState(QuestStepStates.PODE_FINALIZAR), currentIndex - 1);
        }

        SaveAllQuests();
    }


    private void FinishedQuests(string id)
    {
        Quests quests = GetQuestById(id);

        // Salva o estado da última etapa como FINALIZADO
        int lastIndex = quests.CurrentStepIndex >= quests.info.questsEtapasPrefabs.Length
            ? quests.info.questsEtapasPrefabs.Length - 1
            : quests.CurrentStepIndex;

        quests.StoreQuestStepState(new QuestsStepState(QuestStepStates.FINALIZADO), lastIndex);

        // Restante do código...
        GameObject grupoParaRemover = questGrupos
            .FirstOrDefault(g => g.GetComponentInChildren<TextMeshProUGUI>()?.text.Trim() == quests.info.NomeQuest.Trim());

        if (grupoParaRemover != null)
        {
            questGrupos.Remove(grupoParaRemover);
            Destroy(grupoParaRemover);
        }

        QuestsInstancias = QuestsInstancias.Where(q => q != quests.info).ToArray();

        ClaimRewards(quests);
        ChangeQuestState(quests.info.id, QuestsState.FINALIZADO);
        SaveAllQuests();
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
                IdQuests.Add(quest.id, new Quests(quest));
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

    // Estrutura para serializar o progresso das quests
    [System.Serializable]
    public class QuestSaveData
    {
        public List<string> questIDs = new List<string>();
        public List<int> questStepIndices = new List<int>();
        public List<string> questStepStates = new List<string>();
        public List<string> questStates = new List<string>();
    }

    public void SaveAllQuests()
    {
        var currentData = SaveData.Instance;

        currentData.questData.questIDs.Clear();
        currentData.questData.questStepIndices.Clear();
        currentData.questData.questStepStates.Clear();
        currentData.questData.questStates.Clear();

        foreach (var quest in questsDictionary.Values)
        {
            currentData.questData.questIDs.Add(quest.info.id);
            currentData.questData.questStepIndices.Add(quest.CurrentStepIndex);

            // Salva o estado do step atual.
            currentData.questData.questStepStates.Add(quest.GetStepStateSafe());
            currentData.questData.questStates.Add(quest.state.ToString());
        }

        SaveManager.Save(currentData, GameManager.currentSaveSlot);
    }

    public void LoadAllQuests()
    {
        QuestSaveData saveData = SaveData.Instance.questData;

        if (saveData == null || saveData.questIDs.Count == 0)
            return;

        for (int i = 0; i < saveData.questIDs.Count; i++)
        {
            string questID = saveData.questIDs[i];
            int stepIndex = saveData.questStepIndices[i];
            string stepState = saveData.questStepStates[i];
            string questStateStr = saveData.questStates[i];

            if (questsDictionary.ContainsKey(questID))
            {
                var quest = questsDictionary[questID];

                // Restaura o estado da quest
                if (Enum.TryParse<QuestsState>(questStateStr, out QuestsState loadedState))
                {
                    quest.state = loadedState;
                }

                // Restaura o step atual e seu estado
                quest.LoadStep(stepIndex, stepState);

                // Se a quest estiver em andamento, instancia o step na cena
                if (quest.state == QuestsState.EM_ANDAMENTO && quest.CurrentStepExists())
                {
                    quest.InstantiateCurrentStep(this.transform);
                }

                // Atualiza a UI se necessário
                GameManager.instance.questEvents.QuestStateChange(quest);
            }
            else
            {
                Debug.LogWarning($"Quest ID não encontrada ao carregar: {questID}");
            }
        }
    }
}
