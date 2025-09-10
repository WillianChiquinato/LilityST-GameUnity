using UnityEngine;

public class Quests
{
    public QuestsInfoSO info;
    public QuestsState state;
    private int currentStepIndex;

    private QuestsStepState[] stepStates;

    public Quests(QuestsInfoSO questinfo)
    {
        this.info = questinfo;
        this.state = QuestsState.REQUISITOS;
        this.currentStepIndex = 0;
        this.stepStates = new QuestsStepState[info.questsEtapasPrefabs.Length];

        for (int i = 0; i < stepStates.Length; i++)
        {
            stepStates[i] = new QuestsStepState();
        }
    }

    public Quests(QuestsInfoSO questInfo, QuestsState questsState, int currentQuestStepIndex, QuestsStepState[] questStepStates)
    {
        this.info = questInfo;
        this.state = questsState;
        this.currentStepIndex = currentQuestStepIndex;
        this.stepStates = questStepStates;

        if (this.stepStates.Length != this.info.questsEtapasPrefabs.Length)
        {
            Debug.LogWarning("O numero de etapas salvas é diferente do numero de etapas da quest");
        }
    }

    public void MoveParaProximoPasso()
    {
        currentStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return (currentStepIndex < info.questsEtapasPrefabs.Length);
    }

    public void InstantiateCurrentStep(Transform parentTransform)
    {
        GameObject questsStepPreab = GetCurrentStepPrefab();
        if (questsStepPreab != null)
        {
            QuestStep questStep = Object.Instantiate<GameObject>(questsStepPreab, parentTransform).GetComponent<QuestStep>();
            questStep.InitializeQuestStep(info.id, currentStepIndex, stepStates[currentStepIndex].state);
        }
    }

    private GameObject GetCurrentStepPrefab()
    {
        GameObject questsStepPreab = null;
        if (CurrentStepExists())
        {
            questsStepPreab = info.questsEtapasPrefabs[currentStepIndex];
        }
        else
        {
            Debug.LogWarning("Deu bom, não existe mais etapas para essa quest");
        }
        return questsStepPreab;
    }

    public void StoreQuestStepState(QuestsStepState stepState, int stepIndex)
    {
        if (stepIndex < stepStates.Length)
        {
            stepStates[stepIndex].state = stepState.state;
        }
        else
        {
            Debug.Log("Nao tem acesso a etapas");
        }
    }

        public int CurrentStepIndex => currentStepIndex;

        public string GetStepState()
        {
            if (currentStepIndex < stepStates.Length)
                return stepStates[currentStepIndex].state;
            return string.Empty;
        }

        // Restaura o progresso de uma etapa específica
        public void LoadStep(int stepIndex, string stepState)
        {
            if (stepIndex < stepStates.Length)
            {
                currentStepIndex = stepIndex;
                stepStates[stepIndex].state = stepState;
            }
        }

    public QuestData GetQuestData()
    {
        return new QuestData(state, currentStepIndex, stepStates);
    }
}
