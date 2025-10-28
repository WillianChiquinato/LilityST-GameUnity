using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{
    public static InfoManager instance;
    public List<InfoData> infosObtidas = new List<InfoData>();

    [Header("Info instances")]
    private Dictionary<string, InfoData> infoDictionary;
    public InfoPoint[] infoPoints;
    public ScriptableObject[] infoInstancias;
    public List<GameObject> infoGrupos;

    public GameObject instanciaInfoPrefab;
    private GameObject instanciaInfo;
    public GameObject GrupoInfo;

    [Header("UI ContainerDescrição")]
    public GameObject InfoContainer;
    public TextMeshProUGUI titleText;
    public Image IlustracaoImage;
    public TextMeshProUGUI descriptionText;
    public Button BotaoInfo;

    [Header("UI InfoReferencia")]
    public GameObject InfoReferenciaContainer;
    public GameObject ReferenciaInfo;
    public Button ButtonConfirmInfo;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InfoFloatingIcon.Instance.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }

        InfoReferenciaContainer.SetActive(false);

        infoDictionary = CreateInfoMap();
        infoPoints = FindObjectsByType<InfoPoint>(FindObjectsSortMode.None);
        infoGrupos = GameObject.FindGameObjectsWithTag("InfoContainer").OrderBy(info => ExtractNumberFromName(info.name)).ToList();

        infoInstancias = new ScriptableObject[infoGrupos.Count];

        int index = 0;
        foreach (InfoData info in infoDictionary.Values)
        {
            if (index >= infoGrupos.Count)
            {
                break;
            }

            // Armazena a informação no array de informações iniciais
            infoInstancias[index] = info;
            GameManager.instance.questEvents.StartQuest(info.id);

            if (index < infoGrupos.Count)
            {
                GameObject questGrupo = infoGrupos[index];

                TextMeshProUGUI titleQuest = questGrupo.GetComponentInChildren<TextMeshProUGUI>();
                Image imageQuest = questGrupo.GetComponentsInChildren<Image>().FirstOrDefault(img => img.gameObject != questGrupo);

                if (titleQuest != null && imageQuest != null)
                {
                    titleQuest.text = info.InfoName;
                    imageQuest.sprite = info.Icon;
                }
            }

            index++;
        }
    }

    int ExtractNumberFromName(string questName)
    {
        // Usa Regex para encontrar o primeiro número na string.
        // Usado apenas para instanciar o QuestContainer.
        Match match = Regex.Match(questName, @"\d+");

        if (match.Success && int.TryParse(match.Value, out int number))
        {
            return number;
        }

        return 0;
    }

    [Serializable]
    public class InfoSaveData
    {
        public List<string> infoIDs = new List<string>();
        public List<string> infoNome = new List<string>();
        public List<CategoriasInfo> infoCategorias = new List<CategoriasInfo>();
        public List<string> infoDescricoes = new List<string>();
    }

    void Start()
    {
        LoadAllInfos();
    }

    public void AdicionarInfo(InfoData novaInfo)
    {
        if (!infosObtidas.Exists(i => i.id == novaInfo.id))
        {
            infosObtidas.Add(novaInfo);
            novaInfo.obtida = true;

            InfoFloatingIcon.Instance.gameObject.SetActive(true);
            InfoFloatingIcon.Instance.MostrarIcone(novaInfo.InfoName);

            SaveAllInfos();
        }
    }

    private Dictionary<string, InfoData> CreateInfoMap()
    {
        InfoData[] TodasInfos = Resources.LoadAll<InfoData>("Infos");

        Dictionary<string, InfoData> IdInfos = new Dictionary<string, InfoData>();
        foreach (InfoData info in TodasInfos)
        {
            if (IdInfos.ContainsKey(info.id))
            {
                Debug.LogWarning("Já existe uma info com esse id: " + info.id);
            }
            else
            {
                IdInfos.Add(info.id, info);
            }
        }
        return IdInfos;
    }

    void Update()
    {
        for (int i = 0; i < infoGrupos.Count; i++)
        {
            GameObject obj = infoGrupos[i];
            BotaoInfo = obj.GetComponent<Button>();

            if (BotaoInfo != null && i < infoInstancias.Length)
            {
                int index = i;
                InfoData infoData = (InfoData)infoInstancias[index];

                BotaoInfo.onClick.RemoveAllListeners();
                BotaoInfo.onClick.AddListener(() => BotaoInfoDescricoes(infoData, index));
            }
            else
            {
                Debug.LogWarning("O GameObject " + obj.name + " não tem um componente Button ou índice inválido.");
            }
        }

        for (int i = 0; i < infoPoints.Length; i++)
        {
            if (GameManager.instance.playerMoviment.entrar && !infoPoints[i].PlayerAtivo)
            {
                //Se ja existir, nao atualizar a UI.
                if (infosObtidas.Exists(info => info.id == infoPoints[i].infoData.id))
                {
                    continue;
                }
                StartCoroutine(AtualizarUIQuest(infoPoints[i]));
            }
        }
    }

    IEnumerator AtualizarUIQuest(InfoPoint infoPoint)
    {
        var info = GetInfoById(infoPoint.infoData.id);

        if (instanciaInfo == null)
        {
            instanciaInfo = Instantiate(instanciaInfoPrefab, GrupoInfo.transform);
            infoGrupos.Add(instanciaInfo);

            // Adiciona o novo ScriptableObject ao array de QuestsInstancias
            List<ScriptableObject> tempList = infoInstancias.ToList();
            tempList.Add(infoPoint.infoData);
            infoInstancias = tempList.ToArray();

            TextMeshProUGUI titleQuest = instanciaInfo.GetComponentInChildren<TextMeshProUGUI>();
            Image imageInfo = instanciaInfo.GetComponentsInChildren<Image>().FirstOrDefault(img => img.gameObject != instanciaInfo);

            if (titleQuest != null)
            {
                titleQuest.text = infoPoint.infoData.InfoName;
            }
        }

        yield return new WaitForSeconds(1f);

        instanciaInfo = null;
    }

    public void BotaoInfoDescricoes(InfoData infoData, int index)
    {
        if (infoData != null)
        {
            Sprite questIlustracao = infoData.Ilustration;
            string questDescription = infoData.Description;

            if (titleText != null && IlustracaoImage != null)
            {
                titleText.text = infoData.InfoName;
                IlustracaoImage.sprite = questIlustracao;
                IlustracaoImage.color = Color.white;
            }
            if (descriptionText != null)
            {
                descriptionText.text = questDescription;
            }
        }
    }

    private InfoData GetInfoById(string id)
    {
        InfoData info = infoDictionary[id];
        if (info == null)
        {
            Debug.LogWarning("Não existe info com esse id: " + id);

        }
        return info;
    }

    public void SaveAllInfos()
    {
        var currentData = SaveData.Instance;

        currentData.infoData.infoIDs.Clear();
        currentData.infoData.infoNome.Clear();
        currentData.infoData.infoDescricoes.Clear();

        foreach (var info in infosObtidas)
        {
            currentData.infoData.infoIDs.Add(info.id);
            currentData.infoData.infoNome.Add(info.InfoName);
            currentData.infoData.infoCategorias.Add(info.categoriasInfo);
            currentData.infoData.infoDescricoes.Add(info.Description);
        }

        SaveManager.Save(currentData, GameManager.currentSaveSlot);
    }

    public void LoadAllInfos()
    {
        InfoSaveData saveData = SaveData.Instance.infoData;

        if (saveData == null || saveData.infoIDs == null || saveData.infoIDs.Count == 0)
            return;

        infosObtidas.Clear();

        int count = Mathf.Min(
            saveData.infoIDs.Count,
            saveData.infoNome.Count,
            saveData.infoCategorias.Count,
            saveData.infoDescricoes.Count
        );

        for (int i = 0; i < count; i++)
        {
            string id = saveData.infoIDs[i];

            if (infoDictionary.TryGetValue(id, out InfoData originalInfo))
            {
                originalInfo.obtida = true;
                infosObtidas.Add(originalInfo);
            }
            else
            {
                InfoData novaInfo = ScriptableObject.CreateInstance<InfoData>();
                novaInfo.id = id;
                novaInfo.InfoName = saveData.infoNome[i];
                novaInfo.categoriasInfo = saveData.infoCategorias[i];
                novaInfo.Description = saveData.infoDescricoes[i] ?? "";
                novaInfo.obtida = true;
                infosObtidas.Add(novaInfo);

                Debug.LogWarning($"[InfoManager] Info '{id}' não encontrada nos Resources. Criado placeholder temporário.");
            }
        }

        AtualizarUIInfos();

        Debug.Log($"[InfoManager] {infosObtidas.Count} informações carregadas com sucesso.");
    }

    private void AtualizarUIInfos()
    {
        foreach (Transform child in GrupoInfo.transform)
            Destroy(child.gameObject);

        infoGrupos.Clear();
        List<ScriptableObject> tempList = new List<ScriptableObject>();

        foreach (var info in infosObtidas)
        {
            GameObject novoGrupo = Instantiate(instanciaInfoPrefab, GrupoInfo.transform);
            infoGrupos.Add(novoGrupo);
            tempList.Add(info);

            TextMeshProUGUI titleQuest = novoGrupo.GetComponentInChildren<TextMeshProUGUI>();
            Image imageQuest = novoGrupo.GetComponentsInChildren<Image>().FirstOrDefault(img => img.gameObject != novoGrupo);

            if (titleQuest != null)
                titleQuest.text = info.InfoName;

            if (imageQuest != null && info.Icon != null)
                imageQuest.sprite = info.Icon;
        }

        infoInstancias = tempList.ToArray();
    }
}