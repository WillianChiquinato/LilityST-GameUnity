using UnityEngine;

public class TesteDerrotarPrefab : QuestStep
{
    public int inimigosDerrotados = 0;
    private int inimigosNecessarios = 2;

    private void OEnable()
    {
        Debug.Log("Ativando o evento de cacador derrotado.");
    }

    private void ODisable()
    {
        Debug.Log("Desativando o evento de cacador derrotado.");
    }

    public void OnInimigosDerrotado()
    {
        if (inimigosDerrotados < inimigosNecessarios)
        {
            inimigosDerrotados++;
            UpdateState();
        }

        if (inimigosDerrotados >= inimigosNecessarios)
        {
            QuestFinalizadaStep();
        }
    }

    private void UpdateState()
    {
        string state = inimigosDerrotados.ToString();
        ChangeState(state);
    }

    protected override void StoreQuestStepState(string state)
    {
        if (!string.IsNullOrEmpty(state) && int.TryParse(state, out int value))
        {
            this.inimigosDerrotados = value;
        }
        else
        {
            this.inimigosDerrotados = 0;
            
        }
    }
}
