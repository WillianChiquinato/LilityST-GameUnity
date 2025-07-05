using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ArmasSystem : MonoBehaviour
{
    [System.Serializable]
    public class DeckPorArma
    {
        public string armaNome;
        public List<FragmentoItem> fragmentos = new List<FragmentoItem>();
    }

    public List<DeckPorArma> DecksPorArma = new List<DeckPorArma>();

    public static ArmasSystem instance;

    public int maxDeckSize = 5;
    public List<FragmentoData> deck = new List<FragmentoData>();

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

    public bool AdicionarAoDeck(FragmentoData item)
    {
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

        if (!PodeAdicionarFragmento(item))
        {
            // A função interna já mostra a mensagem de erro apropriada
            return false;
        }

        deck.Add(item);
        AtualizarDeckUI(item);
        return true;
    }

    public bool PodeAdicionarFragmento(FragmentoData item)
    {
        int countTempo = deck.Count(c => c.TipoFragmento == fragmentoType.Tempo);
        int countMovimento = deck.Count(c => c.TipoFragmento == fragmentoType.Movimento);
        int countVida = deck.Count(c => c.TipoFragmento == fragmentoType.Vida);
        int countCaos = deck.Count(c => c.TipoFragmento == fragmentoType.Caos);
        int countOrdem = deck.Count(c => c.TipoFragmento == fragmentoType.Ordem);

        return item.TipoFragmento switch
        {
            fragmentoType.Tempo => countTempo < 6,
            fragmentoType.Movimento => countMovimento < 6,
            fragmentoType.Vida => countVida < 6,
            fragmentoType.Caos => countCaos < 6,
            fragmentoType.Ordem => countOrdem < 6,
            _ => true
        };
    }

    public void AtualizarDeckUI(FragmentoData item)
    {
        Debug.Log("Deck Atualizado: " + string.Join(", ", deck.Select(c => c.NomeFragmento)));

        FragmentoSystem.instance.AddToDeckBuilder(item);
    }

    public int GetPrimeiroSlotVazioOuFragmentoExistente(FragmentoData fragData)
    {
        if (deck.Contains(fragData))
            return -1;

        for (int i = 0; i < maxDeckSize; i++)
        {
            if (i >= deck.Count || deck[i] == null)
                return i;
        }

        // Deck cheio
        return -1;
    }
}
