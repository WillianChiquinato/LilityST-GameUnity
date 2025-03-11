using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [Header("Configs Load")]
    [SerializeField] private bool loadQuests = false;
    private int currentPlayerLevel;

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
        questsDictionary = CreateQuestMap();
        questPoints = FindObjectsByType<QuestPoint>(FindObjectsSortMode.None);
        questGrupos = GameObject.FindGameObjectsWithTag("QuestContainer").OrderBy(quest => ExtractNumberFromName(quest.name)).ToList();


        // Conta quantas quests são iniciais
        int count = 0;
        foreach (Quests quest in questsDictionary.Values)
        {
            if (quest.info.StartQuest)
                count++;
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
                Sistema_Pause.instance.questEvents.StartQuest(quest.info.id);
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
        // Aqui extraímos o número da string, assumindo que o nome segue o padrão "questXX"
        string numberPart = new string(questName.Where(char.IsDigit).ToArray());
        return int.Parse(numberPart);
    }

    void OnEnable()
    {
        if (Sistema_Pause.instance == null)
        {
            Debug.LogError("Sistema_Pause não foi inicializado! Aguarde.");
            return;
        }

        Sistema_Pause.instance.questEvents.OnStartQuest += StartQuests;
        Sistema_Pause.instance.questEvents.OnAdvancedQuest += AdvancedQuests;
        Sistema_Pause.instance.questEvents.OnFinishedQuest += FinishedQuests;

        Sistema_Pause.instance.questEvents.OnQuestStepStateChanged += QuestStepStateChange;

        Sistema_Pause.instance.onPlayerLevelChange += PlayerLevelChange;
    }

    void OnDisable()
    {
        Sistema_Pause.instance.questEvents.OnStartQuest -= StartQuests;
        Sistema_Pause.instance.questEvents.OnAdvancedQuest -= AdvancedQuests;
        Sistema_Pause.instance.questEvents.OnFinishedQuest -= FinishedQuests;

        Sistema_Pause.instance.questEvents.OnQuestStepStateChanged += QuestStepStateChange;

        Sistema_Pause.instance.onPlayerLevelChange += PlayerLevelChange;
    }

    void Start()
    {
        foreach (Quests quest in questsDictionary.Values)
        {
            if (quest.state == QuestsState.EM_ANDAMENTO)
            {
                quest.InstantiateCurrentStep(this.transform);
            }
            Sistema_Pause.instance.questEvents.QuestStateChange(quest);
        }
    }

    private void ChangeQuestState(string id, QuestsState newState)
    {
        Quests quests = GetQuestById(id);
        quests.state = newState;
        Sistema_Pause.instance.questEvents.QuestStateChange(quests);
    }

    private void PlayerLevelChange(int level)
    {
        currentPlayerLevel = level;
    }

    private bool CheckRequirementsMet(Quests quest)
    {
        bool meetsRequirements = true;

        // check player level requirements
        if (currentPlayerLevel < quest.info.levelRequisitos)
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
            if (Sistema_Pause.instance.playerMoviment.entrar && !questPoints[i].PlayerAtivo)
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

            if (titleQuest != null && imageQuest != null)
            {
                titleQuest.text = questPoint.questInfopoint.NomeQuest;
                imageQuest.sprite = questPoint.questInfopoint.Icon;
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
        ClaimRewards(quests);
        ChangeQuestState(quests.info.id, QuestsState.FINALIZADO);
    }

    private void ClaimRewards(Quests quest)
    {
        Debug.Log("Parabens, voce ganhou uma dedada no cuzinho!!");
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

    private void OnApplicationQuit()
    {
        foreach (Quests quest in questsDictionary.Values)
        {
            SaveQuestData(quest);
        }
    }

    private void SaveQuestData(Quests quests)
    {
        try
        {
            QuestData questData = quests.GetQuestData();
            string serializedData = JsonUtility.ToJson(questData);
            PlayerPrefs.SetString(quests.info.id, serializedData);
            Debug.Log("Salvando dados da quest: " + serializedData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao salvar dados da quest " + e.Message);
        }
    }

    private Quests LoadQuest(QuestsInfoSO questsInfoSO)
    {
        Quests quests = null;
        try
        {
            if (PlayerPrefs.HasKey(questsInfoSO.id) && loadQuests)
            {
                string serializedData = PlayerPrefs.GetString(questsInfoSO.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quests = new Quests(questsInfoSO, questData.state, questData.questStepIndex, questData.questStepStates);
            }
            else
            {
                quests = new Quests(questsInfoSO);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao carregar dados da quest " + e.Message);
            return null;
        }
        return quests;
    }
}
