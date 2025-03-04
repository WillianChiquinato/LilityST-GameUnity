using System;
using UnityEngine;

public class QuestEvents
{
    public event Action<string> OnStartQuest;
    public void StartQuest(string id)
    {
        if (OnStartQuest != null)
        {
            OnStartQuest(id);
        }
    }

    public event Action<string> OnAdvancedQuest;
    public void AdvancedQuest(string id)
    {
        if (OnAdvancedQuest != null)
        {
            OnAdvancedQuest(id);
        }
    }

    public event Action<string> OnFinishedQuest;
    public void FinishedQuest(string id)
    {
        if (OnFinishedQuest != null)
        {
            OnFinishedQuest(id);
        }
    }

    public event Action<Quests> OnQuestStateChanged;
    public void QuestStateChange(Quests quests)
    {
        if (OnQuestStateChanged != null)
        {
            OnQuestStateChanged(quests);
        }
    }

    public event Action<string, int, QuestsStepState> OnQuestStepStateChanged;
    public void QuestStepStateChange(string id, int stepIndex, QuestsStepState questStepState)
    {
        if (OnQuestStepStateChanged != null)
        {
            OnQuestStepStateChanged(id, stepIndex, questStepState);
        }
    }
}
