using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmasSystem : MonoBehaviour
{
    public static ArmasSystem instance;

    public int maxDeckSize = 5;
    public List<FragmentoData> deck = new List<FragmentoData>();

    private List<FragmentoData> tempDeckItems = new List<FragmentoData>();

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

    void Update()
    {
        AtualizarDeckPlayer();
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
        if (deck.Contains(item))
        {
            Debug.Log("Essa carta já está no deck.");
            return false;
        }

        int countTempo = deck.Count(c => c.TipoFragmento == fragmentoType.Tempo);
        int countMovimento = deck.Count(c => c.TipoFragmento == fragmentoType.Movimento);
        int countVida = deck.Count(c => c.TipoFragmento == fragmentoType.Vida);
        int countCaos = deck.Count(c => c.TipoFragmento == fragmentoType.Caos);
        int countOrdem = deck.Count(c => c.TipoFragmento == fragmentoType.Ordem);

        return PodeAdicionarFragmento(item, countTempo, countMovimento, countVida, countCaos, countOrdem);
    }

    public bool PodeAdicionarFragmento(FragmentoData item, int countTempo, int countMovimento, int countVida, int countCaos, int countOrdem)
    {
        switch (item.TipoFragmento)
        {
            case fragmentoType.Tempo:
                if (countTempo >= 1)
                {
                    Debug.Log("Você só pode adicionar até 1 cartas cinzas.");
                    return false;
                }
                break;
            case fragmentoType.Movimento:
                if (countMovimento >= 1)
                {
                    Debug.Log("Você só pode adicionar até 1 cartas douradas.");
                    return false;
                }
                break;
            case fragmentoType.Vida:
                if (countVida >= 1)
                {
                    Debug.Log("Você só pode adicionar 1 carta espectral.");
                    return false;
                }
                break;

            case fragmentoType.Caos:
                if (countCaos >= 1)
                {
                    Debug.Log("Você só pode adicionar 1 carta vermelha.");
                    return false;
                }
                break;

            case fragmentoType.Ordem:
                if (countOrdem >= 1)
                {
                    Debug.Log("Você só pode adicionar 1 carta azul.");
                    return false;
                }
                break;
        }
        return true;
    }

    public void AtualizarDeckUI(FragmentoData item)
    {
        Debug.Log("Deck Atualizado: " + string.Join(", ", deck.Select(c => c.NomeFragmento)));

        FragmentoSystem.instance.AddToDeckBuilder(item);
    }

    public void AtualizarDeckPlayer()
    {
        //Atualizar atributos das armas.
    }
}
