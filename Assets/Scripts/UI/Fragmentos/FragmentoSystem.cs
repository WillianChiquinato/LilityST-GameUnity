using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FragmentoSystem : MonoBehaviour
{
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
    [SerializeField] private Transform TempoSlotParent;
    [SerializeField] private Transform MovimentoSlotParent;
    [SerializeField] private Transform VidaSlotParent;
    [SerializeField] private Transform CaosSlotParent;
    [SerializeField] private Transform OrdemSlotParent;
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
        InicializarDecks();
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
            SeletorArmas.instance.SelectArma(0);
        }
    }

    void InicializarDecks()
    {
        string[] armas = new[] { "Bastão", "Arco", "Marreta", "Luva", "Mascara", "Sino" };
        foreach (var arma in armas)
        {
            if (!DecksPorArma.Any(d => d.armaNome == arma))
            {
                DecksPorArma.Add(new DeckPorArmaSaveData { armaNome = arma });
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
        foreach (var deck in DecksPorArma)
        {
            DeckPorArmaSaveData deckSave = new DeckPorArmaSaveData();
            deckSave.armaNome = deck.armaNome;

            foreach (var frag in deck.fragmentos)
            {
                if (frag == null)
                {
                    Debug.LogWarning($"Fragmento inválido no deck '{deck.armaNome}', ignorando.");
                    continue;
                }

                // aqui você monta o save data a partir do FragmentoItem
                deckSave.fragmentos.Add(new FragmentoItemSaveData
                {
                    fragmentoNome = frag.fragmentoNome,
                    fragmentoType = frag.fragmentoType,
                    stackSize = frag.stackSize
                });
            }

            saveData.DecksPorArma.Add(deckSave);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.dataPath + "/Scripts/SaveData/Inventario/fragmentos.json", json);

        Debug.Log("Fragmentos e decks salvos em: " + Application.dataPath + "/Scripts/SaveData/Inventario/fragmentos.json");
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

        foreach (var deckSave in FragmentosInventoryData.DecksPorArma)
        {
            DeckPorArmaSaveData deckData = new DeckPorArmaSaveData
            {
                armaNome = deckSave.armaNome,
                fragmentos = new List<FragmentoItemSaveData>()
            };

            foreach (var frag in deckSave.fragmentos)
            {
                FragmentoData fragData = GetFragmentoData(frag.fragmentoNome, frag.fragmentoType);
                if (fragData != null)
                {
                    deckData.fragmentos.Add(frag);
                }
                else
                {
                    Debug.LogWarning($"Fragmento '{frag.fragmentoNome}' não encontrado para deck '{deckSave.armaNome}'");
                }
            }

            DecksPorArma.Add(deckData);
        }

        UpdateInventory();
        string armaSelecionada = FragmentosInventoryData.armaSelecionada;

        if (string.IsNullOrEmpty(armaSelecionada))
            armaSelecionada = "Bastão";

        Debug.Log($"Carregando arma selecionada: {armaSelecionada}");
        SelecionarArma(armaSelecionada);
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
            Debug.LogWarning($"Deck para arma '{armaNome}' não encontrado.");
            return;
        }

        DeckBuilder.Clear();
        DeckBuilderDicionary.Clear();
        ArmasSystem.instance.deck.Clear();

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

                    // ✅ Atualiza também as listas corretas
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
    }


    public void UpdateDeckUI(string armaNome)
    {
        // Busca os dados do deck para a arma selecionada
        var deck = DecksPorArma.FirstOrDefault(d => d.armaNome == armaNome);

        if (deck == null)
        {
            Debug.LogWarning($"Nenhum deck encontrado para arma: {armaNome}");
            return;
        }

        Debug.Log($"Atualizando UI para arma: {armaNome}");

        // slots visuais
        for (int i = 0; i < DeckBuilderSlotParent.childCount; i++)
        {
            var slotTransform = DeckBuilderSlotParent.GetChild(i);
            var slotUI = slotTransform.GetComponent<FragmentosSlot_UI>();

            if (i < deck.fragmentos.Count)
            {
                var fragSaveData = deck.fragmentos[i];
                var fragData = GetFragmentoData(fragSaveData.fragmentoNome, fragSaveData.fragmentoType);

                if (fragData != null)
                {
                    var fragmentoItem = new FragmentoItem(fragData)
                    {
                        stackSize = fragSaveData.stackSize
                    };

                    slotUI.UpdateInventory(fragmentoItem);
                    Debug.Log($"Slot {i}: {fragData.NomeFragmento} x{fragSaveData.stackSize}");
                }
                else
                {
                    Debug.LogWarning($"Fragmento não encontrado nos Resources: {fragSaveData.fragmentoNome}");
                    slotUI.CleanUpSlot();
                }
            }
            else
            {
                // Não há fragmento para este slot, limpa.
                slotUI.CleanUpSlot();
            }
        }

        Debug.Log($"Atualizando deck UI para arma: '{armaNome}'");
        foreach (var decks in DecksPorArma)
        {
            Debug.Log($"Deck disponível: '{decks.armaNome}' com {decks.fragmentos.Count} fragmentos");
        }
    }

}
