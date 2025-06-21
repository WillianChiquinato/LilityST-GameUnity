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
        this.PontosCheckPoint = System.Int32.Parse(state);
        UpdateState();
    }
}
