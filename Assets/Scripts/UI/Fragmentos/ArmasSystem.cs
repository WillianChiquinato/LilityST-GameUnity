using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FragmentoSystem;

[DefaultExecutionOrder(-1000)]
public class ArmasSystem : MonoBehaviour
{
    public fragmentoSaveData saveData;

    public static ArmasSystem instance;

    public int maxDeckSize = 5;
    public List<FragmentoData> deck = new List<FragmentoData>();

    public Dictionary<string, List<FragmentoData>> decksPorArmaRuntime = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AdicionarAoDeck(string arma, FragmentoData item)
    {
        var deck = decksPorArmaRuntime[arma];

        if (deck.Contains(item))
        {
            Debug.Log("Essa carta já está no deck.");
            return false;
        }

        if (deck.Count >= maxDeckSize)
        {
            Debug.Log("Deck está cheio!");
            return false;
        }

        if (!PodeAdicionarFragmento(deck, item))
        {
            Debug.Log("Você atingiu o limite para esse tipo de fragmento.");
            return false;
        }

        deck.Add(item);
        AtualizarDeckUI(arma);
        return true;
        
    }
    

    public bool PodeAdicionarFragmento(List<FragmentoData> deck, FragmentoData item)
    {
        int count = deck.Count(c => c.TipoFragmento == item.TipoFragmento);
        return count < 6;
    }

    public void AtualizarDeckUI(string arma)
    {
        var deck = decksPorArmaRuntime[arma];
        FragmentoSystem.instance.UpdateDeckUI(arma, deck);
    }

    public int GetPrimeiroSlotVazioOuFragmentoExistente(List<FragmentoData> deck, FragmentoData fragmento)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            if (deck[i] != null && deck[i] == fragmento)
            {
                return i;
            }
        }

        for (int i = 0; i < deck.Count; i++)
        {
            if (deck[i] == null)
            {
                return i;
            }
        }

        return -1;
    }
}