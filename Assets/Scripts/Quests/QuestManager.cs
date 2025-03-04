using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Configs Load")]
    [SerializeField] private bool loadQuests = false;

    private Dictionary<string, Quests> questsDictionary;

    private int currentPlayerLevel;

    void Awake()
    {
        questsDictionary = CreateQuestMap();
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
        foreach (Quests quest in questsDictionary.Values)
        {
            if (quest.state == QuestsState.REQUISITOS && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestsState.PODE_INICIAR);
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
