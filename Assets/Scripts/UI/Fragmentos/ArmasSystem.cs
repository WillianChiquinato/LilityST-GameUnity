using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FragmentoSystem;

[DefaultExecutionOrder(-1000)]
public class ArmasSystem : MonoBehaviour
{
    public static ArmasSystem instance;

    public int maxDeckSize = 5;
    public List<FragmentoData> deck = new List<FragmentoData>();

    public Dictionary<string, List<FragmentoData>> decksPorArmaRuntime = new();
    public string armaSelecionada = "Bastão";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InicializarDecksRuntime();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InicializarDecksRuntime()
    {
        string[] armas = new[] { "Bastão", "Arco", "Marreta", "Luva", "Mascara", "Sino" };
        foreach (var arma in armas)
        {
            if (!decksPorArmaRuntime.ContainsKey(arma))
            {
                decksPorArmaRuntime[arma] = new List<FragmentoData>();
            }
        }
    }

    public bool AdicionarAoDeck(string arma, FragmentoData item)
    {
        if (!decksPorArmaRuntime.ContainsKey(arma))
        {
            return false;
        }

        var deck = decksPorArmaRuntime[arma];

        if (deck.Contains(item))
        {
            Debug.Log("Essa carta já está no deck.");
            ToastMessage.Instance.ShowToast("Fragmento já está no Deck!", ToastType.Alert);
            return false;
        }

        if (deck.Count >= maxDeckSize)
        {
            Debug.Log($"Deck está cheio! ({deck.Count}/{maxDeckSize})");
            ToastMessage.Instance.ShowToast("Deck está cheio!", ToastType.Alert);
            return false;
        }

        if (!PodeAdicionarFragmento(deck, item))
        {
            Debug.Log("Você atingiu o limite para esse tipo de fragmento.");
            return false;
        }

        ToastMessage.Instance.ShowToast("Fragmento Adicionado ao Deck!", ToastType.Success);
        deck.Add(item);
        armaSelecionada = arma;

        // Também adicionar aos dados salvos
        var deckSalvo = FragmentoSystem.instance.DecksPorArma.FirstOrDefault(d => d.armaNome == arma);
        if (deckSalvo == null)
        {
            deckSalvo = new DeckPorArmaSaveData { armaNome = arma };
            FragmentoSystem.instance.DecksPorArma.Add(deckSalvo);
        }

        var novoFragmentoSave = new FragmentoItemSaveData
        {
            fragmentoNome = item.NomeFragmento,
            fragmentoType = item.TipoFragmento,
            stackSize = 1
        };

        deckSalvo.fragmentos.Add(novoFragmentoSave);
        AtualizarDeckUI(arma);

        Debug.Log($"Fragmento '{item.NomeFragmento}' adicionado com sucesso ao deck da arma '{arma}'");
        return true;
    }


    public bool PodeAdicionarFragmento(List<FragmentoData> deck, FragmentoData item)
    {
        int count = deck.Count(c => c.TipoFragmento == item.TipoFragmento);
        return count < 6;
    }

    public void AtualizarDeckUI(string arma)
    {
        if (!decksPorArmaRuntime.ContainsKey(arma))
        {
            return;
        }

        var deck = decksPorArmaRuntime[arma];
        armaSelecionada = arma;
        FragmentoSystem.instance.UpdateDeckUI(arma, deck);
    }

    public int GetPrimeiroSlotVazioOuFragmentoExistente(List<FragmentoData> deck, FragmentoData fragmento)
    {
        // Verificar se o fragmento já existe no deck
        for (int i = 0; i < deck.Count; i++)
        {
            if (deck[i] != null && deck[i] == fragmento)
            {
                return i; // Retorna o índice onde o fragmento já existe
            }
        }

        if (deck.Count < maxDeckSize)
        {
            return deck.Count;
        }

        return -1;
    }

    public bool RemoverDoDeck(string arma, FragmentoData item)
    {
        if (!decksPorArmaRuntime.ContainsKey(arma))
        {
            Debug.LogError($"Arma '{arma}' não encontrada nos decks runtime!");
            return false;
        }

        var deck = decksPorArmaRuntime[arma];
        bool removido = deck.Remove(item);

        if (removido)
        {
            // Também remover dos dados salvos
            var deckSalvo = FragmentoSystem.instance.DecksPorArma.FirstOrDefault(d => d.armaNome == arma);
            if (deckSalvo != null)
            {
                var fragmentoParaRemover = deckSalvo.fragmentos
                    .FirstOrDefault(f => f.fragmentoNome == item.NomeFragmento && f.fragmentoType == item.TipoFragmento);

                if (fragmentoParaRemover != null)
                {
                    deckSalvo.fragmentos.Remove(fragmentoParaRemover);
                }
                else
                {
                    Debug.LogWarning($"Fragmento '{item.NomeFragmento}' não encontrado nos dados salvos para remoção");
                }
            }
            else
            {
                Debug.LogError($"Deck salvo não encontrado para arma '{arma}'");
            }

            // Sincronizar dados salvos com runtime no FragmentoSystem
            FragmentoSystem.instance.SelecionarArma(arma);
            FragmentoSystem.instance.SaveFragment();
        }
        else
        {
            Debug.LogWarning($"Falha ao remover fragmento '{item.NomeFragmento}' do deck runtime da arma '{arma}'");
        }

        return removido;
    }
}