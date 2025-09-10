using UnityEngine;

public static class QuestStepStates
{
    public const string INICIADO = "INICIADO";
    public const string FINALIZADO = "FINALIZADO";
    public const string PODE_FINALIZAR = "PODE_FINALIZAR";
    public const string EM_ANDAMENTO = "EM_ANDAMENTO";
    public const string PENDENTE = "";
}

[System.Serializable]
public class QuestsStepState
{
    public string state;

    public QuestsStepState(string newState)
    {
        this.state = newState;
    }

    public QuestsStepState()
    {
        this.state = "";
    }
}
