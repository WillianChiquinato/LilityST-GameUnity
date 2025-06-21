using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] public QuestsInfoSO questInfopoint;

    [Header("Configurações")]
    [SerializeField] private bool Startpoint = true;
    [SerializeField] private bool Endpoint = true;
    public bool pointStarted = false;

    public bool PlayerAtivo = false;
    private bool PlayerEstaPerto = false;
    public string QuestId;
    public QuestsState CurrentQuestState;
    public QuestIcon questIcon;

    void Awake()
    {
        PlayerAtivo = false;
        PlayerEstaPerto = false;

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
        Debug.Log("SubmitPressed chamado!");
        if (!PlayerEstaPerto)
        {
            return;
        }
        PlayerAtivo = true;

        if (!pointStarted)
        {
            var state = CurrentQuestState;

            if (state == QuestsState.PODE_INICIAR && Startpoint)
            {
                Sistema_Pause.instance.questEvents.StartQuest(QuestId);
            }
            else if (state == QuestsState.PODE_FINALIZAR && Endpoint)
            {
                Sistema_Pause.instance.questEvents.FinishedQuest(QuestId);
            }
        }

        pointStarted = true;
        QuestManager.instance.SaveAllQuests();
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
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerEstaPerto = true;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Sistema_Pause.instance.player.entrar && !pointStarted)
            {
                Debug.Log("SubmitPressed adicionado!");
                SubmitPressed();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!pointStarted)
            {
                PlayerEstaPerto = false;
                PlayerAtivo = false;    
            }
        }
    }
}
