using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestsInfoSO questInfopoint;

    [Header("Configurações")]
    [SerializeField] private bool Startpoint = true;
    [SerializeField] private bool Endpoint = true;

    public bool PlayerAtivo = false;
    private bool PlayerEstaPerto = false;
    private string QuestId;
    private QuestsState CurrentQuestState;

    private QuestIcon questIcon;

    void Awake()
    {
        QuestId = questInfopoint.id;
        questIcon = GetComponentInChildren<QuestIcon>();
    }

    void OnEnable()
    {
        if (Sistema_Pause.instance != null)
        {
            Sistema_Pause.instance.questEvents.OnQuestStateChanged += QuestStateChanged;
        }
    }

    private void SubmitPressed()
    {
        if (!PlayerEstaPerto)
        {
            return;
        }

        if(CurrentQuestState == QuestsState.PODE_INICIAR && Startpoint)
        {
            Sistema_Pause.instance.questEvents.StartQuest(QuestId);
        }
        else if (CurrentQuestState == QuestsState.PODE_FINALIZAR && Endpoint)
        {
            Sistema_Pause.instance.questEvents.FinishedQuest(QuestId);
        }

        Debug.Log($"QuestPoint {QuestId} foi ativado!");
    }

    void OnDisable()
    {
        if (Sistema_Pause.instance != null)
        {
            Sistema_Pause.instance.questEvents.OnQuestStateChanged -= QuestStateChanged;
        }
    }

    private void QuestStateChanged(Quests quests)
    {
        if (quests.info.id.Equals(QuestId))
        {
            CurrentQuestState = quests.state;
            questIcon.SetState(CurrentQuestState, Startpoint, Endpoint);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerEstaPerto = true;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Sistema_Pause.instance.playerMoviment.entrar && !PlayerAtivo)
            {
                PlayerAtivo = true;
                SubmitPressed();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerEstaPerto = false;
            PlayerAtivo = false;
        }
    }
}
