using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public QuestsState state;
    public int questStepIndex;
    public QuestsStepState[] questStepStates;

    public QuestData(QuestsState state, int questStepIndex, QuestsStepState[] questStepStates)
    {
        this.state = state;
        this.questStepIndex = questStepIndex;
        this.questStepStates = questStepStates;
    }
}
