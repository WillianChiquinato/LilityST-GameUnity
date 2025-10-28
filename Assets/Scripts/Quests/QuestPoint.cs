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
        if (GameManager.instance != null)
        {
            GameManager.instance.questEvents.OnQuestStateChanged += QuestStateChanged;
        }
    }

    private void SubmitPressed()
    {
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
                GameManager.instance.questEvents.StartQuest(QuestId);
            }
            else if (state == QuestsState.PODE_FINALIZAR && Endpoint)
            {
                GameManager.instance.questEvents.FinishedQuest(QuestId);
            }

            // Salva só depois que a quest foi realmente iniciada/avançada
            QuestManager.instance.SaveAllQuests();
            pointStarted = true;
        }
    }

    void OnDisable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.questEvents.OnQuestStateChanged -= QuestStateChanged;
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
            if (GameManager.instance.player.entrar && !pointStarted)
            {
                if (CurrentQuestState.Equals(QuestsState.FINALIZADO))
                {
                    return;
                }
                
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
