using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FragmentoSystem : MonoBehaviour
{
    public GameObject backContentLivro;
    public List<DeckPorArmaSaveData> DecksPorArma = new();
    public static FragmentoSystem instance;

    public List<FragmentoData> startEquipament;
    public bool inicializacaoItens = false;

    public List<FragmentoItem> ChaveTempo;
    public Dictionary<FragmentoData, FragmentoItem> ChaveTempoDicionary;

    public List<FragmentoItem> ChaveMovimento;
    public Dictionary<FragmentoData, FragmentoItem> ChaveMovimentoDicionary;

    public List<FragmentoItem> ChaveVida;
    public Dictionary<FragmentoData, FragmentoItem> ChaveVidaDicionary;

    public List<FragmentoItem> ChaveCaos;
    public Dictionary<FragmentoData, FragmentoItem> ChaveCaosDicionary;

    public List<FragmentoItem> ChaveOrdem;
    public Dictionary<FragmentoData, FragmentoItem> ChaveOrdemDicionary;

    //Formação do Deck Builder.
    public List<FragmentoItem> DeckBuilder;
    public Dictionary<FragmentoData, FragmentoItem> DeckBuilderDicionary;


    [Header("Inventory UI")]
    [SerializeField] public Transform TempoSlotParent;
    [SerializeField] public Transform MovimentoSlotParent;
    [SerializeField] public Transform VidaSlotParent;
    [SerializeField] public Transform CaosSlotParent;
    [SerializeField] public Transform OrdemSlotParent;
    [SerializeField] public Transform DeckBuilderSlotParent;

    [SerializeField] private FragmentosSlot_UI[] TempoItemSlot;
    [SerializeField] private FragmentosSlot_UI[] MovimentoItemSlot;
    [SerializeField] private FragmentosSlot_UI[] VidaItemSlot;
    [SerializeField] private FragmentosSlot_UI[] CaosItemSlot;
    [SerializeField] private FragmentosSlot_UI[] OrdemItemSlot;

    [SerializeField] private FragmentosSlot_UI[] DeckBuilderItemSlot;


    void Awake()
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

    void Start()
    {
        backContentLivro.SetActive(false);

        ChaveTempo = new List<FragmentoItem>();
        ChaveTempoDicionary = new Dictionary<FragmentoData, FragmentoItem>();

        ChaveMovimento = new List<FragmentoItem>();
        ChaveMovimentoDicionary = new Dictionary<FragmentoData, FragmentoItem>();

        ChaveVida = new List<FragmentoItem>();
        ChaveVidaDicionary = new Dictionary<FragmentoData, FragmentoItem>();

        ChaveCaos = new List<FragmentoItem>();
        ChaveCaosDicionary = new Dictionary<FragmentoData, FragmentoItem>();

        ChaveOrdem = new List<FragmentoItem>();
        ChaveOrdemDicionary = new Dictionary<FragmentoData, FragmentoItem>();

        DeckBuilder = new List<FragmentoItem>();
        DeckBuilderDicionary = new Dictionary<FragmentoData, FragmentoItem>();

        TempoItemSlot = TempoSlotParent.GetComponentsInChildren<FragmentosSlot_UI>();
        MovimentoItemSlot = MovimentoSlotParent.GetComponentsInChildren<FragmentosSlot_UI>();
        VidaItemSlot = VidaSlotParent.GetComponentsInChildren<FragmentosSlot_UI>();
        CaosItemSlot = CaosSlotParent.GetComponentsInChildren<FragmentosSlot_UI>();
        OrdemItemSlot = OrdemSlotParent.GetComponentsInChildren<FragmentosSlot_UI>();

        DeckBuilderItemSlot = DeckBuilderSlotParent.GetComponentsInChildren<FragmentosSlot_UI>();

        // Inicializar decks
        InicializarDecks();

        int currentSlot = GameManager.currentSaveSlot;

        var loadedData = SaveManager.Load(currentSlot);
        if (loadedData != null)
        {
            SaveData.Instance = loadedData;
        }

        if (!SaveData.Instance.fragmentoData.isInitialized)
        {
            foreach (var item in startEquipament)
            {
                AddItem(item);
            }

            inicializacaoItens = true;
            SaveFragment();
            Debug.Log("Fragmento inicializado com os itens de startEquipament.");
        }
        else
        {
            LoadFragment();
        }

        // Garantir que uma arma esteja selecionada
        if (string.IsNullOrEmpty(ArmasSystem.instance.armaSelecionada))
        {
            ArmasSystem.instance.armaSelecionada = "Bastão";
            SeletorArmas.instance.currentIndex = 0;
        }
    }

    void InicializarDecks()
    {
        string[] armas = new[] { "Bastão", "Arco", "Marreta", "Luva", "Mascara", "Sino" };

        // Inicializar DecksPorArma para salvar dados
        foreach (var arma in armas)
        {
            if (!DecksPorArma.Any(d => d.armaNome == arma))
            {
                DecksPorArma.Add(new DeckPorArmaSaveData { armaNome = arma });
            }
        }

        // Garantir que o ArmasSystem também tenha os decks inicializados
        foreach (var arma in armas)
        {
            if (!ArmasSystem.instance.decksPorArmaRuntime.ContainsKey(arma))
            {
                ArmasSystem.instance.decksPorArmaRuntime[arma] = new List<FragmentoData>();
            }
        }
    }

    //Save e Load das cartas no JSON
    [System.Serializable]
    public class FragmentoItemSaveData
    {
        public string fragmentoNome;
        public fragmentoType fragmentoType;
        public int stackSize;
    }

    private FragmentoSaveData saveData;
    [System.Serializable]
    public class FragmentoSaveData
    {
        public List<FragmentoItemSaveData> ChaveTempo = new List<FragmentoItemSaveData>();
        public List<FragmentoItemSaveData> ChaveMovimento = new List<FragmentoItemSaveData>();
        public List<FragmentoItemSaveData> ChaveVida = new List<FragmentoItemSaveData>();
        public List<FragmentoItemSaveData> ChaveCaos = new List<FragmentoItemSaveData>();
        public List<FragmentoItemSaveData> ChaveOrdem = new List<FragmentoItemSaveData>();

        public List<DeckPorArmaSaveData> DecksPorArma = new();
        public string armaSelecionada = "Bastão";
        public bool isInitialized = false;
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < TempoItemSlot.Length; i++)
        {
            TempoItemSlot[i].CleanUpSlot();
        }
        for (int i = 0; i < MovimentoItemSlot.Length; i++)
        {
            MovimentoItemSlot[i].CleanUpSlot();
        }
        for (int i = 0; i < VidaItemSlot.Length; i++)
        {
            VidaItemSlot[i].CleanUpSlot();
        }
        for (int i = 0; i < CaosItemSlot.Length; i++)
        {
            CaosItemSlot[i].CleanUpSlot();
        }
        for (int i = 0; i < OrdemItemSlot.Length; i++)
        {
            OrdemItemSlot[i].CleanUpSlot();
        }



        for (int i = 0; i < DeckBuilderItemSlot.Length; i++)
        {
            DeckBuilderItemSlot[i].CleanUpSlot();
        }


        for (int i = 0; i < ChaveTempo.Count; i++)
        {
            TempoItemSlot[i].UpdateInventory(ChaveTempo[i]);
        }
        for (int i = 0; i < ChaveMovimento.Count; i++)
        {
            MovimentoItemSlot[i].UpdateInventory(ChaveMovimento[i]);
        }
        for (int i = 0; i < ChaveVida.Count; i++)
        {
            VidaItemSlot[i].UpdateInventory(ChaveVida[i]);
        }
        for (int i = 0; i < ChaveCaos.Count; i++)
        {
            CaosItemSlot[i].UpdateInventory(ChaveCaos[i]);
        }
        for (int i = 0; i < ChaveOrdem.Count; i++)
        {
            OrdemItemSlot[i].UpdateInventory(ChaveOrdem[i]);
        }


        for (int i = 0; i < DeckBuilder.Count; i++)
        {
            DeckBuilderItemSlot[i].UpdateInventory(DeckBuilder[i]);
        }
    }

    public void AddItem(FragmentoData _item)
    {
        switch (_item.TipoFragmento)
        {
            case fragmentoType.Tempo:
                AddToTempo(_item);
                break;
            case fragmentoType.Movimento:
                AddToMovimento(_item);
                break;
            case fragmentoType.Vida:
                AddToVida(_item);
                break;
            case fragmentoType.Caos:
                AddToCaos(_item);
                break;
            case fragmentoType.Ordem:
                AddToOrdem(_item);
                break;
        }

        UpdateInventory();
    }

    public void AddToTempo(FragmentoData _item)
    {
        // Cada fragmento é único - sempre criar novo item
        FragmentoItem newItem = new FragmentoItem(_item);
        ChaveTempo.Add(newItem);
        // Não usar dicionário para evitar sobrescrever fragmentos únicos
    }

    public void AddToMovimento(FragmentoData _item)
    {
        // Cada fragmento é único - sempre criar novo item
        FragmentoItem newItem = new FragmentoItem(_item);
        ChaveMovimento.Add(newItem);
        // Não usar dicionário para evitar sobrescrever fragmentos únicos
    }

    public void AddToVida(FragmentoData _item)
    {
        // Cada fragmento é único - sempre criar novo item
        FragmentoItem newItem = new FragmentoItem(_item);
        ChaveVida.Add(newItem);
        // Não usar dicionário para evitar sobrescrever fragmentos únicos
    }

    public void AddToCaos(FragmentoData _item)
    {
        // Cada fragmento é único - sempre criar novo item
        FragmentoItem newItem = new FragmentoItem(_item);
        ChaveCaos.Add(newItem);
        // Não usar dicionário para evitar sobrescrever fragmentos únicos
    }

    public void AddToOrdem(FragmentoData _item)
    {
        // Cada fragmento é único - sempre criar novo item
        FragmentoItem newItem = new FragmentoItem(_item);
        ChaveOrdem.Add(newItem);
        // Não usar dicionário para evitar sobrescrever fragmentos únicos
    }

    public void AddToDeckBuilder(FragmentoData _item)
    {
        FragmentoItem newItem = new FragmentoItem(_item);
        DeckBuilder.Add(newItem);
        DeckBuilderDicionary.Add(_item, newItem);

        UpdateInventory();
    }

    public void RemoveItem(FragmentoData _item)
    {
        // Para fragmentos únicos, procurar e remover primeira ocorrência

        // Procurar em Tempo
        var tempoItem = ChaveTempo.FirstOrDefault(item => item.FragmentoData == _item);
        if (tempoItem != null)
        {
            ChaveTempo.Remove(tempoItem);
            UpdateInventory();
            return;
        }

        // Procurar em Movimento
        var movimentoItem = ChaveMovimento.FirstOrDefault(item => item.FragmentoData == _item);
        if (movimentoItem != null)
        {
            ChaveMovimento.Remove(movimentoItem);
            UpdateInventory();
            return;
        }

        // Procurar em Vida
        var vidaItem = ChaveVida.FirstOrDefault(item => item.FragmentoData == _item);
        if (vidaItem != null)
        {
            ChaveVida.Remove(vidaItem);
            UpdateInventory();
            return;
        }

        // Procurar em Caos
        var caosItem = ChaveCaos.FirstOrDefault(item => item.FragmentoData == _item);
        if (caosItem != null)
        {
            ChaveCaos.Remove(caosItem);
            UpdateInventory();
            return;
        }

        // Procurar em Ordem
        var ordemItem = ChaveOrdem.FirstOrDefault(item => item.FragmentoData == _item);
        if (ordemItem != null)
        {
            ChaveOrdem.Remove(ordemItem);
            UpdateInventory();
            return;
        }

        Debug.LogWarning($"Fragmento {_item.NomeFragmento} não encontrado no inventário para remoção");
        UpdateInventory();
    }

    public void SaveFragment()
    {
        Debug.Log("SaveFragment iniciado");
        var currentData = SaveData.Instance;

        if (currentData.fragmentoData.DecksPorArma == null || currentData.fragmentoData.DecksPorArma.Count == 0)
        {
            currentData.fragmentoData.DecksPorArma = new List<DeckPorArmaSaveData>(DecksPorArma);
        }

        // Salvar os fragmentos gerais - cada fragmento é único
        currentData.fragmentoData.ChaveTempo.Clear();
        foreach (var item in ChaveTempo)
        {
            currentData.fragmentoData.ChaveTempo.Add(new FragmentoItemSaveData
            {
                fragmentoNome = item.FragmentoData.NomeFragmento,
                fragmentoType = item.FragmentoData.TipoFragmento,
                stackSize = 1 // Sempre 1 para fragmentos únicos
            });
        }

        currentData.fragmentoData.ChaveMovimento.Clear();
        foreach (var item in ChaveMovimento)
        {
            currentData.fragmentoData.ChaveMovimento.Add(new FragmentoItemSaveData
            {
                fragmentoNome = item.FragmentoData.NomeFragmento,
                fragmentoType = item.FragmentoData.TipoFragmento,
                stackSize = 1 // Sempre 1 para fragmentos únicos
            });
        }

        currentData.fragmentoData.ChaveVida.Clear();
        foreach (var item in ChaveVida)
        {
            currentData.fragmentoData.ChaveVida.Add(new FragmentoItemSaveData
            {
                fragmentoNome = item.FragmentoData.NomeFragmento,
                fragmentoType = item.FragmentoData.TipoFragmento,
                stackSize = 1 // Sempre 1 para fragmentos únicos
            });
        }

        currentData.fragmentoData.ChaveCaos.Clear();
        foreach (var item in ChaveCaos)
        {
            currentData.fragmentoData.ChaveCaos.Add(new FragmentoItemSaveData
            {
                fragmentoNome = item.FragmentoData.NomeFragmento,
                fragmentoType = item.FragmentoData.TipoFragmento,
                stackSize = 1 // Sempre 1 para fragmentos únicos
            });
        }

        currentData.fragmentoData.ChaveOrdem.Clear();
        foreach (var item in ChaveOrdem)
        {
            currentData.fragmentoData.ChaveOrdem.Add(new FragmentoItemSaveData
            {
                fragmentoNome = item.FragmentoData.NomeFragmento,
                fragmentoType = item.FragmentoData.TipoFragmento,
                stackSize = 1 // Sempre 1 para fragmentos únicos
            });
        }

        // Salvar os decks por arma - usar apenas os dados do DecksPorArma como fonte de verdade
        foreach (var deck in currentData.fragmentoData.DecksPorArma)
        {
            var atualizado = DecksPorArma.FirstOrDefault(d => d.armaNome == deck.armaNome);
            if (atualizado != null)
            {
                deck.fragmentos = new List<FragmentoItemSaveData>(atualizado.fragmentos);
            }
        }

        // Salvar a arma selecionada atual
        currentData.fragmentoData.armaSelecionada = ArmasSystem.instance.armaSelecionada;
        currentData.fragmentoData.isInitialized = inicializacaoItens;

        SaveManager.Save(currentData, GameManager.currentSaveSlot);
    }

    public void LoadFragment()
    {
        FragmentoSaveData data = SaveData.Instance.fragmentoData;

        // Limpar inventário antes de carregar
        ChaveTempo.Clear();
        ChaveMovimento.Clear();
        ChaveVida.Clear();
        ChaveCaos.Clear();
        ChaveOrdem.Clear();

        // Limpar e reinicializar os decks runtime
        ArmasSystem.instance.decksPorArmaRuntime.Clear();
        string[] armas = new[] { "Bastão", "Arco", "Marreta", "Luva", "Mascara", "Sino" };
        foreach (var arma in armas)
        {
            ArmasSystem.instance.decksPorArmaRuntime[arma] = new List<FragmentoData>();
        }

        // Garante que DecksPorArma tem os 6 decks base
        var baseDecks = new List<DeckPorArmaSaveData>();
        foreach (var arma in armas)
        {
            var deck = data.DecksPorArma.FirstOrDefault(d => d.armaNome == arma);
            if (deck == null)
            {
                deck = new DeckPorArmaSaveData { armaNome = arma, fragmentos = new List<FragmentoItemSaveData>() };
            }
            baseDecks.Add(deck);
        }
        DecksPorArma = baseDecks;

        // Função para adicionar fragmentos ao inventário
        void AddStack(FragmentoItemSaveData itemData)
        {
            FragmentoData item = GetFragmentoData(itemData.fragmentoNome, itemData.fragmentoType);
            if (item != null)
            {
                for (int i = 0; i < itemData.stackSize; i++)
                {
                    AddItem(item);
                }
            }
            else
            {
                Debug.LogWarning($"Fragmento '{itemData.fragmentoNome}' não encontrado.");
            }
        }

        // Adicionar fragmentos salvos
        foreach (var frag in data.ChaveTempo) AddStack(frag);
        foreach (var frag in data.ChaveMovimento) AddStack(frag);
        foreach (var frag in data.ChaveVida) AddStack(frag);
        foreach (var frag in data.ChaveCaos) AddStack(frag);
        foreach (var frag in data.ChaveOrdem) AddStack(frag);

        // Carregar os decks para runtime
        foreach (var deckSave in DecksPorArma)
        {
            var deckRuntime = new List<FragmentoData>();
            foreach (var frag in deckSave.fragmentos)
            {
                FragmentoData fragData = GetFragmentoData(frag.fragmentoNome, frag.fragmentoType);
                if (fragData != null)
                {
                    deckRuntime.Add(fragData);
                }
            }
            ArmasSystem.instance.decksPorArmaRuntime[deckSave.armaNome] = deckRuntime;
        }

        UpdateInventory();

        // Carregar a arma selecionada
        string armaSelecionada = string.IsNullOrEmpty(data.armaSelecionada) ? "Bastão" : data.armaSelecionada;
        ArmasSystem.instance.armaSelecionada = armaSelecionada;

        // Atualizar UI do seletor de armas
        int indiceArma = System.Array.IndexOf(SeletorArmas.instance.nomesDasArmas, armaSelecionada);
        if (indiceArma >= 0)
            SeletorArmas.instance.currentIndex = indiceArma;

        SelecionarArma(armaSelecionada);
        InicializarDecks();
        saveData = data;
    }

    private FragmentoData GetFragmentoData(string ChaveName, fragmentoType type)
    {
        Debug.Log("Tentando carregar: Fragmentos/" + ChaveName);
        FragmentoData fragmento = Resources.Load<FragmentoData>("Fragmentos/" + ChaveName);
        if (fragmento == null)
        {
            Debug.LogError("Item não encontrado: " + ChaveName);
        }
        return fragmento;
    }

    [System.Serializable]
    public class DeckPorArmaSaveData
    {
        public string armaNome;
        public List<FragmentoItemSaveData> fragmentos = new();
    }

    public void SelecionarArma(string armaNome)
    {
        var deck = DecksPorArma.FirstOrDefault(d => d.armaNome == armaNome);
        if (deck == null)
        {
            Debug.LogWarning($"Deck para arma '{armaNome}' não encontrado nos dados salvos. Criando novo deck vazio.");
            deck = new DeckPorArmaSaveData { armaNome = armaNome };
            DecksPorArma.Add(deck);
        }

        // Verificar se existe no runtime e sincronizar
        if (!ArmasSystem.instance.decksPorArmaRuntime.ContainsKey(armaNome))
        {
            ArmasSystem.instance.decksPorArmaRuntime[armaNome] = new List<FragmentoData>();
        }

        // Sincronizar dados salvos com runtime
        var deckRuntime = ArmasSystem.instance.decksPorArmaRuntime[armaNome];
        deckRuntime.Clear();

        // Limpar o DeckBuilder UI
        DeckBuilder.Clear();
        DeckBuilderDicionary.Clear();
        ArmasSystem.instance.deck.Clear();

        // Carregar fragmentos do deck salvo para o runtime e UI
        for (int i = 0; i < deck.fragmentos.Count; i++)
        {
            var fragSave = deck.fragmentos[i];
            FragmentoData fragData = GetFragmentoData(fragSave.fragmentoNome, fragSave.fragmentoType);

            if (fragData != null)
            {
                // Adicionar ao runtime
                deckRuntime.Add(fragData);

                // Criar item para UI - cada fragmento é único
                FragmentoItem fragItem = new FragmentoItem(fragData);
                fragItem.stackSize = 1; // Sempre 1 para fragmentos únicos

                DeckBuilder.Add(fragItem);
                if (!DeckBuilderDicionary.ContainsKey(fragData))
                {
                    DeckBuilderDicionary.Add(fragData, fragItem);
                }

                ArmasSystem.instance.deck.Add(fragData);

                Debug.Log($"Carregado para deck '{armaNome}': {fragSave.fragmentoNome}");
            }
        }

        // Atualizar UI dos slots
        for (int i = 0; i < DeckBuilderItemSlot.Length; i++)
        {
            if (i < DeckBuilder.Count)
            {
                DeckBuilderItemSlot[i].UpdateInventory(DeckBuilder[i]);
            }
            else
            {
                DeckBuilderItemSlot[i].CleanUpSlot();
            }
        }

        // Sincronizar com o ArmasSystem
        ArmasSystem.instance.armaSelecionada = armaNome;

        Debug.Log($"Arma '{armaNome}' selecionada com {deckRuntime.Count} fragmentos sincronizados.");
    }

    public void UpdateDeckUI(string armaNome, List<FragmentoData> deckAtual)
    {
        Debug.Log($"UpdateDeckUI chamado - Atualizando UI para arma: {armaNome} com {deckAtual.Count} fragmentos");

        // Sincronizar DeckBuilder com o deck atual
        DeckBuilder.Clear();
        DeckBuilderDicionary.Clear();

        for (int i = 0; i < DeckBuilderSlotParent.childCount; i++)
        {
            var slotTransform = DeckBuilderSlotParent.GetChild(i);
            var slotUI = slotTransform.GetComponent<FragmentosSlot_UI>();

            if (i < deckAtual.Count)
            {
                var fragData = deckAtual[i];

                if (fragData != null)
                {
                    var fragmentoItem = new FragmentoItem(fragData) { stackSize = 1 };

                    // Adicionar ao DeckBuilder para manter sincronização
                    DeckBuilder.Add(fragmentoItem);
                    if (!DeckBuilderDicionary.ContainsKey(fragData))
                    {
                        DeckBuilderDicionary.Add(fragData, fragmentoItem);
                    }

                    slotUI.UpdateInventory(fragmentoItem);
                }
                else
                {
                    slotUI.CleanUpSlot();
                    Debug.Log($"Slot {i}: fragmento nulo, limpando slot");
                }
            }
            else
            {
                slotUI.CleanUpSlot();
                Debug.Log($"Slot {i}: sem fragmento, limpando slot");
            }
        }

        Debug.Log($"UpdateDeckUI concluído para arma {armaNome}");
    }

    public bool AdicionarFragmentoAoDeckAtual(FragmentoData fragmento)
    {
        string armaAtual = ArmasSystem.instance.armaSelecionada;

        DeckPorArmaSaveData deckArma = DecksPorArma
            .FirstOrDefault(d => d.armaNome == armaAtual);

        if (deckArma == null)
        {
            // Criar um novo deck se não existir
            deckArma = new DeckPorArmaSaveData { armaNome = armaAtual };
            DecksPorArma.Add(deckArma);
        }

        if (deckArma.fragmentos.Count >= ArmasSystem.instance.maxDeckSize)
        {
            Debug.LogWarning("Deck está cheio!");
            return false;
        }

        var fragmentoSaveData = new FragmentoItemSaveData
        {
            fragmentoNome = fragmento.NomeFragmento,
            fragmentoType = fragmento.TipoFragmento,
            stackSize = 1
        };

        deckArma.fragmentos.Add(fragmentoSaveData);

        // Adicionar ao runtime também
        if (!ArmasSystem.instance.decksPorArmaRuntime.ContainsKey(armaAtual))
        {
            ArmasSystem.instance.decksPorArmaRuntime[armaAtual] = new List<FragmentoData>();
        }

        ArmasSystem.instance.decksPorArmaRuntime[armaAtual].Add(fragmento);
        ArmasSystem.instance.AtualizarDeckUI(armaAtual);
        return true;
    }
}
