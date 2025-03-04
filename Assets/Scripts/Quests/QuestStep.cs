using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool finalizado = false;
    private string questID;
    private int stepIndex;


    public void InitializeQuestStep(string questID, int currentStepIndex, string questStepState)
    {
        this.questID = questID;
        this.stepIndex = currentStepIndex;
        if(questStepState != null && questStepState != "")
        {
            StoreQuestStepState(questStepState);
        }
    }

    protected void QuestFinalizadaStep()
    {
        if (!finalizado)
        {
            finalizado = true;
            Sistema_Pause.instance.questEvents.AdvancedQuest(questID);
            Destroy(this.gameObject, 0.3f);
        }
    }

    protected void ChangeState(string newState)
    {
        Sistema_Pause.instance.questEvents.QuestStepStateChange(questID, stepIndex, new QuestsStepState(newState));
    }

    protected abstract void StoreQuestStepState(string state);
}
