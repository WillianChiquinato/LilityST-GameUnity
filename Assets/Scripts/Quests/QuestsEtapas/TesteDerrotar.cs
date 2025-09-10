using System.Collections.Generic;
using UnityEngine;

public class TesteDerrotar : MonoBehaviour
{
    public QuestPoint questPointManual;
    public TesteDerrotarPrefab Instancia;
    public List<GameObject> inimigosNoTrigger = new List<GameObject>();

    void Start()
    {
        if (questPointManual.CurrentQuestState == QuestsState.FINALIZADO)
        {
            this.GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, 1f);
        }
    }

    void Update()
    {
        if (Instancia != null)
        {
            Debug.Log("CheckPointQuest encontrado!");
        }
        else
        {
            Debug.Log("CheckPointQuest não encontrado!");
            Instancia = GameObject.FindFirstObjectByType<TesteDerrotarPrefab>();
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Inimigos") && !inimigosNoTrigger.Contains(other.gameObject))
        {
            inimigosNoTrigger.Add(other.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Inimigos"))
        {
            bool todosMortos = true;

            foreach (var inimigo in inimigosNoTrigger)
            {
                if (inimigo != null)
                {
                    var inimigoVida = inimigo.GetComponent<Damage>();
                    if (inimigoVida != null && inimigoVida.IsAlive)
                    {
                        todosMortos = false;
                        break;
                    }
                }
            }

            if (todosMortos && inimigosNoTrigger.Count > 0)
            {
                inimigosNoTrigger.Clear();

                Instancia.inimigosDerrotados = 2;
                Instancia.OnInimigosDerrotado();
                questPointManual.pointStarted = false;
                Destroy(gameObject, 0.5f);
                Debug.Log("Todos os inimigos DENTRO do trigger foram derrotados!");
            }
        }
    }
}
