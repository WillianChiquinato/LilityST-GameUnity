using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FragmentoSystem : MonoBehaviour
{
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

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.dataPath + "/Scripts/SaveData/Inventario/fragmentos.json", json);

        Debug.Log("Fragmentos salvas em: " + Application.dataPath + "/Scripts/SaveData/Inventario/fragmentos.json");
    }

    public void LoadFragmento()
    {
        string path = Application.dataPath + "/Scripts/SaveData/Inventario/fragmentos.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("Arquivo JSON carregado: " + json);
            fragmentoSaveData FragmentosInventoryData = JsonUtility.FromJson<fragmentoSaveData>(json);

            // Limpar inventário antes de carregar
            ChaveTempo.Clear();
            ChaveMovimento.Clear();
            ChaveVida.Clear();
            ChaveCaos.Clear();
            ChaveOrdem.Clear();
            Debug.Log("CLEAR INVENTORY");

            // Carregar os itens de cada tipo
            foreach (var fragmentoData in FragmentosInventoryData.ChaveTempo)
            {
                FragmentoData item = GetFragmentoData(fragmentoData.fragmentoNome, fragmentoData.fragmentoType);
                if (item != null)
                {
                    for (int i = 0; i < fragmentoData.stackSize; i++)
                    {
                        AddItem(item);
                        Debug.Log("Adicionando item: " + fragmentoData.fragmentoNome + " com quantidade: " + fragmentoData.stackSize);
                    }
                }
            }

            foreach (var fragmentoData in FragmentosInventoryData.ChaveMovimento)
            {
                FragmentoData item = GetFragmentoData(fragmentoData.fragmentoNome, fragmentoData.fragmentoType);
                if (item != null)
                {
                    for (int i = 0; i < fragmentoData.stackSize; i++)
                    {
                        AddItem(item);
                        Debug.Log("Adicionando item: " + fragmentoData.fragmentoNome + " com quantidade: " + fragmentoData.stackSize);
                    }
                }
            }

            foreach (var fragmento in FragmentosInventoryData.ChaveVida)
            {
                FragmentoData item = GetFragmentoData(fragmento.fragmentoNome, fragmento.fragmentoType);
                if (item != null)
                {
                    for (int i = 0; i < fragmento.stackSize; i++)
                    {
                        AddItem(item);
                        Debug.Log("Adicionando item: " + fragmento.fragmentoNome + " com quantidade: " + fragmento.stackSize);
                    }
                }
            }

            foreach (var fragmento in FragmentosInventoryData.ChaveCaos)
            {
                FragmentoData item = GetFragmentoData(fragmento.fragmentoNome, fragmento.fragmentoType);
                if (item != null)
                {
                    for (int i = 0; i < fragmento.stackSize; i++)
                    {
                        AddItem(item);
                        Debug.Log("Adicionando item: " + fragmento.fragmentoNome + " com quantidade: " + fragmento.stackSize);
                    }
                }
            }

            foreach (var fragmento in FragmentosInventoryData.ChaveOrdem)
            {
                FragmentoData item = GetFragmentoData(fragmento.fragmentoNome, fragmento.fragmentoType);
                if (item != null)
                {
                    for (int i = 0; i < fragmento.stackSize; i++)
                    {
                        AddItem(item);
                        Debug.Log("Adicionando item: " + fragmento.fragmentoNome + " com quantidade: " + fragmento.stackSize);
                    }
                }
            }

            UpdateInventory();
        }
        else
        {
            Debug.LogWarning("Arquivo de inventário não encontrado em: " + path);
        }
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
}
