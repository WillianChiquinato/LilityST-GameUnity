using UnityEngine;

public class CheckPointQuest : QuestStep
{
    public int PontosCheckPoint = 0;
    private int PontosCheckPointComplete = 3;

    private void OnEnable()
    {
        //Ativou.
    }

    private void OnDisable()
    {
        //Desativou.
    }

    public void CheckPointCollected()
    {
        if (PontosCheckPoint < PontosCheckPointComplete)
        {
            PontosCheckPoint++;
            UpdateState();
        }

        if (PontosCheckPoint >= PontosCheckPointComplete)
        {
            QuestFinalizadaStep();
        }
    }

    private void UpdateState()
    {
        string state = PontosCheckPoint.ToString();
        ChangeState(state);
    }

    protected override void StoreQuestStepState(string state)
    {
        if (!string.IsNullOrEmpty(state) && int.TryParse(state, out int value))
        {
            this.PontosCheckPoint = value;
        }
        else
        {
            this.PontosCheckPoint = 0;
        }
    }
}
