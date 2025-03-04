using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestIcon : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField] private GameObject IconeRequisitos;
    [SerializeField] private GameObject IconePodeIniciar;
    [SerializeField] private GameObject IconeEmAndamento;
    [SerializeField] private GameObject IconePodeFinalizar;

    public void SetState(QuestsState newState, bool startPoint, bool finishPoint)
    {
        IconeRequisitos.SetActive(false);
        IconePodeIniciar.SetActive(false);
        IconeEmAndamento.SetActive(false);
        IconePodeFinalizar.SetActive(false);

        switch (newState)
        {
            case QuestsState.REQUISITOS:
                if (startPoint) { IconeRequisitos.SetActive(true); }
                break;
            case QuestsState.PODE_INICIAR:
                if (startPoint) { IconePodeIniciar.SetActive(true); }
                break;
            case QuestsState.EM_ANDAMENTO:
                if (finishPoint) { IconeEmAndamento.SetActive(true); }
                break;
            case QuestsState.PODE_FINALIZAR:
                if (finishPoint) { IconePodeFinalizar.SetActive(true); }
                break;
            case QuestsState.FINALIZADO:
                break;
            default:
                Debug.LogWarning("Sem Estados encontrados");
                break;
        }
    }
}
