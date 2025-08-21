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

        if (IsInventoryJSONFileEmpty("Assets/Scripts/SaveData/Inventario/fragmentos.json"))
        {
            foreach (var item in startEquipament)
            {
                AddItem(item);
            }

            SaveFragment();
            Debug.Log("Inventário inicializado com os itens de startEquipament.");
        }
        else
        {
            LoadFragmento();
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

    public bool IsInventoryJSONFileEmpty(string path)
    {
        if (File.Exists(path))
        {
            string fileContent = File.ReadAllText(path);
            return string.IsNullOrWhiteSpace(fileContent);
        }
        else
        {
            Debug.LogError("O arquivo JSON não foi encontrado.");
            return true;
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

    private fragmentoSaveData saveData;
    [System.Serializable]
    public class fragmentoSaveData
    {
        public List<FragmentoItemSaveData> ChaveTempo = new List<FragmentoItemSaveData>();
        public List<FragmentoItemSaveData> ChaveMovimento = new List<FragmentoItemSaveData>();
        public List<FragmentoItemSaveData> ChaveVida = new List<FragmentoItemSaveData>();
        public List<FragmentoItemSaveData> ChaveCaos = new List<FragmentoItemSaveData>();
        public List<FragmentoItemSaveData> ChaveOrdem = new List<FragmentoItemSaveData>();

        public List<DeckPorArmaSaveData> DecksPorArma = new();
        public string armaSelecionada = "Bastão";
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
        if (ChaveTempoDicionary.TryGetValue(_item, out FragmentoItem value))
        {
            value.AddStack();
        }
        else
        {
            FragmentoItem newItem = new FragmentoItem(_item);
            ChaveTempo.Add(newItem);
            ChaveTempoDicionary.Add(_item, newItem);
        }
    }

    public void AddToMovimento(FragmentoData _item)
    {
        if (ChaveMovimentoDicionary.TryGetValue(_item, out FragmentoItem value))
        {
            value.AddStack();
        }
        else
        {
            FragmentoItem newItem = new FragmentoItem(_item);
            ChaveMovimento.Add(newItem);
            ChaveMovimentoDicionary.Add(_item, newItem);
        }
    }

    public void AddToVida(FragmentoData _item)
    {
        if (ChaveVidaDicionary.TryGetValue(_item, out FragmentoItem value))
        {
            value.AddStack();
        }
        else
        {
            FragmentoItem newItem = new FragmentoItem(_item);
            ChaveVida.Add(newItem);
            ChaveVidaDicionary.Add(_item, newItem);
        }
    }

    public void AddToCaos(FragmentoData _item)
    {
        if (ChaveCaosDicionary.TryGetValue(_item, out FragmentoItem value))
        {
            value.AddStack();
        }
        else
        {
            FragmentoItem newItem = new FragmentoItem(_item);
            ChaveCaos.Add(newItem);
            ChaveCaosDicionary.Add(_item, newItem);
        }
    }

    public void AddToOrdem(FragmentoData _item)
    {
        if (ChaveOrdemDicionary.TryGetValue(_item, out FragmentoItem value))
        {
            value.AddStack();
        }
        else
        {
            FragmentoItem newItem = new FragmentoItem(_item);
            ChaveOrdem.Add(newItem);
            ChaveOrdemDicionary.Add(_item, newItem);
        }
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
        if (ChaveTempoDicionary.TryGetValue(_item, out FragmentoItem TempoValue))
        {
            if (TempoValue.stackSize <= 1)
            {
                ChaveTempo.Remove(TempoValue);
                ChaveTempoDicionary.Remove(_item);
            }
            else
            {
                TempoValue.RemoveStack();
            }
        }

        if (ChaveMovimentoDicionary.TryGetValue(_item, out FragmentoItem MovimentoValue))
        {
            if (MovimentoValue.stackSize <= 1)
            {
                ChaveMovimento.Remove(MovimentoValue);
                ChaveMovimentoDicionary.Remove(_item);
            }
            else
            {
                MovimentoValue.RemoveStack();
            }
        }

        if (ChaveVidaDicionary.TryGetValue(_item, out FragmentoItem VidaValue))
        {
            if (VidaValue.stackSize <= 1)
            {
                ChaveVida.Remove(VidaValue);
                ChaveVidaDicionary.Remove(_item);
            }
            else
            {
                VidaValue.RemoveStack();
            }
        }

        if (ChaveCaosDicionary.TryGetValue(_item, out FragmentoItem ChaosValue))
        {
            if (ChaosValue.stackSize <= 1)
            {
                ChaveCaos.Remove(ChaosValue);
                ChaveCaosDicionary.Remove(_item);
            }
            else
            {
                ChaosValue.RemoveStack();
            }
        }

        if (ChaveOrdemDicionary.TryGetValue(_item, out FragmentoItem OrderValue))
        {
            if (OrderValue.stackSize <= 1)
            {
                ChaveOrdem.Remove(OrderValue);
                ChaveOrdemDicionary.Remove(_item);
            }
            else
            {
                OrderValue.RemoveStack();
            }
        }

        UpdateInventory();
    }

    public void SaveFragment()
    {
        Debug.Log("SaveFragment iniciado");
        fragmentoSaveData saveData = new fragmentoSaveData();

        // Salvar os fragmentos gerais
        foreach (var item in ChaveTempo)
        {
            saveData.ChaveTempo.Add(new FragmentoItemSaveData
            {
                fragmentoNome = item.FragmentoData.NomeFragmento,
                fragmentoType = item.FragmentoData.TipoFragmento,
                stackSize = item.stackSize
            });
        }

        foreach (var item in ChaveMovimento)
        {
            saveData.ChaveMovimento.Add(new FragmentoItemSaveData
            {
                fragmentoNome = item.FragmentoData.NomeFragmento,
                fragmentoType = item.FragmentoData.TipoFragmento,
                stackSize = item.stackSize
            });
        }

        foreach (var item in ChaveVida)
        {
            saveData.ChaveVida.Add(new FragmentoItemSaveData
            {
                fragmentoNome = item.FragmentoData.NomeFragmento,
                fragmentoType = item.FragmentoData.TipoFragmento,
                stackSize = item.stackSize
            });
        }

        foreach (var item in ChaveCaos)
        {
            saveData.ChaveCaos.Add(new FragmentoItemSaveData
            {
                fragmentoNome = item.FragmentoData.NomeFragmento,
                fragmentoType = item.FragmentoData.TipoFragmento,
                stackSize = item.stackSize
            });
        }

        foreach (var item in ChaveOrdem)
        {
            saveData.ChaveOrdem.Add(new FragmentoItemSaveData
            {
                fragmentoNome = item.FragmentoData.NomeFragmento,
                fragmentoType = item.FragmentoData.TipoFragmento,
                stackSize = item.stackSize
            });
        }

        // Salvar os decks por arma
        foreach (var kvp in ArmasSystem.instance.decksPorArmaRuntime)
        {
            DeckPorArmaSaveData deckSave = new DeckPorArmaSaveData
            {
                armaNome = kvp.Key,
                fragmentos = kvp.Value.Select(frag => new FragmentoItemSaveData
                {
                    fragmentoNome = frag.NomeFragmento,
                    fragmentoType = frag.TipoFragmento,
                    stackSize = 1
                }).ToList()
            };

            saveData.DecksPorArma.Add(deckSave);
        }

        // Salvar a arma selecionada atual
        saveData.armaSelecionada = ArmasSystem.instance.armaSelecionada;

        string json = JsonUtility.ToJson(saveData, true);
        Debug.Log($"JSON gerado para salvamento: {json.Length} caracteres");

        File.WriteAllText(Application.dataPath + "/Scripts/SaveData/Inventario/fragmentos.json", json);

        Debug.Log("SaveFragment concluído - Fragmentos e decks salvos em: " + Application.dataPath + "/Scripts/SaveData/Inventario/fragmentos.json");
    }

    public void LoadFragmento()
    {
        string path = Application.dataPath + "/Scripts/SaveData/Inventario/fragmentos.json";

        if (!File.Exists(path))
        {
            Debug.LogWarning("Arquivo de inventário não encontrado: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        fragmentoSaveData FragmentosInventoryData = JsonUtility.FromJson<fragmentoSaveData>(json);

        Debug.Log("Arquivo JSON carregado.");

        // Limpar inventário antes de carregar
        ChaveTempo.Clear();
        ChaveMovimento.Clear();
        ChaveVida.Clear();
        ChaveCaos.Clear();
        ChaveOrdem.Clear();
        DecksPorArma.Clear();

        // Limpar e reinicializar os decks runtime
        ArmasSystem.instance.decksPorArmaRuntime.Clear();
        string[] armas = new[] { "Bastão", "Arco", "Marreta", "Luva", "Mascara", "Sino" };
        foreach (var arma in armas)
        {
            ArmasSystem.instance.decksPorArmaRuntime[arma] = new List<FragmentoData>();
        }

        foreach (var deckSave in FragmentosInventoryData.DecksPorArma)
        {
            DeckPorArmaSaveData deckSaveCopy = new DeckPorArmaSaveData();
            deckSaveCopy.armaNome = deckSave.armaNome;
            deckSaveCopy.fragmentos = new List<FragmentoItemSaveData>(deckSave.fragmentos);
            DecksPorArma.Add(deckSaveCopy);
        }

        Debug.Log("Inventário limpo.");

        void AddStack(FragmentoItemSaveData data)
        {
            FragmentoData item = GetFragmentoData(data.fragmentoNome, data.fragmentoType);
            if (item != null)
            {
                for (int i = 0; i < data.stackSize; i++)
                {
                    AddItem(item);
                }
            }
            else
            {
                Debug.LogWarning($"Fragmento '{data.fragmentoNome}' não encontrado.");
            }
        }

        foreach (var frag in FragmentosInventoryData.ChaveTempo) AddStack(frag);
        foreach (var frag in FragmentosInventoryData.ChaveMovimento) AddStack(frag);
        foreach (var frag in FragmentosInventoryData.ChaveVida) AddStack(frag);
        foreach (var frag in FragmentosInventoryData.ChaveCaos) AddStack(frag);
        foreach (var frag in FragmentosInventoryData.ChaveOrdem) AddStack(frag);

        // Carregar os decks por arma
        foreach (var deckSave in FragmentosInventoryData.DecksPorArma)
        {
            List<FragmentoData> deckRuntime = new List<FragmentoData>();

            foreach (var frag in deckSave.fragmentos)
            {
                FragmentoData fragData = GetFragmentoData(frag.fragmentoNome, frag.fragmentoType);
                if (fragData != null)
                {
                    deckRuntime.Add(fragData);
                }
                else
                {
                    Debug.LogWarning($"Fragmento '{frag.fragmentoNome}' não encontrado para deck '{deckSave.armaNome}'");
                }
            }

            ArmasSystem.instance.decksPorArmaRuntime[deckSave.armaNome] = deckRuntime;
        }

        UpdateInventory();

        // Carregar a arma selecionada
        string armaSelecionada = FragmentosInventoryData.armaSelecionada;
        if (string.IsNullOrEmpty(armaSelecionada))
            armaSelecionada = "Bastão";

        ArmasSystem.instance.armaSelecionada = armaSelecionada;

        Debug.Log($"Carregando arma selecionada: {armaSelecionada}");

        // Encontrar o índice da arma selecionada para o SeletorArmas
        int indiceArma = System.Array.IndexOf(SeletorArmas.instance.nomesDasArmas, armaSelecionada);
        if (indiceArma >= 0)
        {
            SeletorArmas.instance.currentIndex = indiceArma;
        }

        SelecionarArma(armaSelecionada);
        InicializarDecks();
        saveData = FragmentosInventoryData;
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

        // Verificar se existe no runtime
        if (!ArmasSystem.instance.decksPorArmaRuntime.ContainsKey(armaNome))
        {
            ArmasSystem.instance.decksPorArmaRuntime[armaNome] = new List<FragmentoData>();
        }

        // Limpar o DeckBuilder UI
        DeckBuilder.Clear();
        DeckBuilderDicionary.Clear();
        ArmasSystem.instance.deck.Clear();

        // Carregar fragmentos do deck salvo para a UI
        for (int i = 0; i < DeckBuilderItemSlot.Length; i++)
        {
            if (i < deck.fragmentos.Count)
            {
                var fragSave = deck.fragmentos[i];
                FragmentoData fragData = GetFragmentoData(fragSave.fragmentoNome, fragSave.fragmentoType);

                if (fragData != null)
                {
                    FragmentoItem fragItem = new FragmentoItem(fragData);
                    fragItem.stackSize = fragSave.stackSize;

                    DeckBuilder.Add(fragItem);
                    if (!DeckBuilderDicionary.ContainsKey(fragData))
                    {
                        DeckBuilderDicionary.Add(fragData, fragItem);
                    }

                    ArmasSystem.instance.deck.Add(fragData);
                    DeckBuilderItemSlot[i].UpdateInventory(fragItem);

                    Debug.Log($"Carregado para deck '{armaNome}': {fragSave.fragmentoNome}");
                }
                else
                {
                    DeckBuilderItemSlot[i].CleanUpSlot();
                }
            }
            else
            {
                DeckBuilderItemSlot[i].CleanUpSlot();
            }
        }

        // Sincronizar com o ArmasSystem
        ArmasSystem.instance.armaSelecionada = armaNome;

        Debug.Log($"Arma '{armaNome}' selecionada com {deck.fragmentos.Count} fragmentos.");
    }


    public void UpdateDeckUI(string armaNome, List<FragmentoData> deckAtual)
    {
        Debug.Log($"UpdateDeckUI chamado - Atualizando UI para arma: {armaNome} com {deckAtual.Count} fragmentos");

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
                    slotUI.UpdateInventory(fragmentoItem);
                    Debug.Log($"Slot {i}: {fragData.NomeFragmento} atualizado");
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

        Debug.Log($"Fragmento '{fragmento.NomeFragmento}' adicionado ao deck de {armaAtual}.");
        return true;
    }
}
