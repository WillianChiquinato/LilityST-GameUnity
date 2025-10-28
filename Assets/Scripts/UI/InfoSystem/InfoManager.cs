using System;
using System.Collections.Generic;
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

            InfoFloatingIcon.Instance.MostrarIcone(novaInfo.InfoName);

            SaveAllInfos();
        }
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

        if (saveData == null || saveData.infoIDs.Count == 0)
            return;

        infosObtidas.Clear();

        for (int i = 0; i < saveData.infoIDs.Count; i++)
        {
            InfoData novaInfo = new InfoData
            {
                id = saveData.infoIDs[i],
                InfoName = saveData.infoNome[i],
                categoriasInfo = saveData.infoCategorias[i],
                Description = saveData.infoDescricoes[i],
                obtida = true
            };

            infosObtidas.Add(novaInfo);
        }
    }
}
