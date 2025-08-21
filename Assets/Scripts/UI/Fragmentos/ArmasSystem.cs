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
        Debug.Log($"ArmasSystem.AdicionarAoDeck chamado - Arma: {arma}, Fragmento: {item.NomeFragmento}");

        if (!decksPorArmaRuntime.ContainsKey(arma))
        {
            Debug.LogError($"Arma '{arma}' não encontrada nos decks runtime!");
            return false;
        }

        var deck = decksPorArmaRuntime[arma];
        Debug.Log($"Deck atual tem {deck.Count} fragmentos");

        if (deck.Contains(item))
        {
            Debug.Log("Essa carta já está no deck.");
            return false;
        }

        if (deck.Count >= maxDeckSize)
        {
            Debug.Log($"Deck está cheio! ({deck.Count}/{maxDeckSize})");
            return false;
        }

        if (!PodeAdicionarFragmento(deck, item))
        {
            Debug.Log("Você atingiu o limite para esse tipo de fragmento.");
            return false;
        }

        Debug.Log($"Adicionando fragmento '{item.NomeFragmento}' ao deck");
        deck.Add(item);
        armaSelecionada = arma;

        // Também adicionar aos dados salvos
        var deckSalvo = FragmentoSystem.instance.DecksPorArma.FirstOrDefault(d => d.armaNome == arma);
        if (deckSalvo == null)
        {
            Debug.Log($"Criando novo deck salvo para arma '{arma}'");
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
        Debug.Log($"Fragmento adicionado aos dados salvos");

        Debug.Log($"Atualizando UI do deck");
        AtualizarDeckUI(arma);

        // Salva automaticamente após adicionar
        Debug.Log($"Salvando fragmentos");
        FragmentoSystem.instance.SaveFragment();

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
            Debug.LogError($"Arma '{arma}' não encontrada nos decks runtime!");
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
        Debug.Log($"RemoverDoDeck chamado - Arma: {arma}, Fragmento: {item.NomeFragmento}");

        if (!decksPorArmaRuntime.ContainsKey(arma))
        {
            Debug.LogError($"Arma '{arma}' não encontrada nos decks runtime!");
            return false;
        }

        var deck = decksPorArmaRuntime[arma];
        Debug.Log($"Deck runtime antes da remoção tem {deck.Count} fragmentos");

        bool removido = deck.Remove(item);
        Debug.Log($"Remoção do deck runtime: {removido}");

        if (removido)
        {
            Debug.Log($"Deck runtime após remoção tem {deck.Count} fragmentos");

            // Também remover dos dados salvos
            var deckSalvo = FragmentoSystem.instance.DecksPorArma.FirstOrDefault(d => d.armaNome == arma);
            if (deckSalvo != null)
            {
                Debug.Log($"Deck salvo antes da remoção tem {deckSalvo.fragmentos.Count} fragmentos");

                var fragmentoParaRemover = deckSalvo.fragmentos
                    .FirstOrDefault(f => f.fragmentoNome == item.NomeFragmento && f.fragmentoType == item.TipoFragmento);

                if (fragmentoParaRemover != null)
                {
                    deckSalvo.fragmentos.Remove(fragmentoParaRemover);
                    Debug.Log($"Fragmento removido dos dados salvos. Deck salvo agora tem {deckSalvo.fragmentos.Count} fragmentos");
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

            Debug.Log($"Atualizando UI do deck após remoção");
            AtualizarDeckUI(arma);

            // Salva automaticamente após remover
            Debug.Log($"Salvando fragmentos após remoção");
            FragmentoSystem.instance.SaveFragment();

            Debug.Log($"Fragmento '{item.NomeFragmento}' removido com sucesso do deck da arma '{arma}'");
        }
        else
        {
            Debug.LogWarning($"Falha ao remover fragmento '{item.NomeFragmento}' do deck runtime da arma '{arma}'");
        }

        return removido;
    }
}