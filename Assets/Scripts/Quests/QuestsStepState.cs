using UnityEngine;

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
