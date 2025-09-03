using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsUI : MonoBehaviour
{
    public int slotIndex;
    public GameObject slotItemConteudo;
    public GameObject slotItemVazio;
    public Sprite[] imagensCenas;
    public Image[] imagensPoderes;

    private void Start()
    {
        UpdateSlotUI();
    }

    public void OnSlotClicked()
    {
        GameManager.currentSaveSlot = slotIndex;

        if (SaveManager.SaveExists(slotIndex))
        {
            // Já tem save → Carregar
            SaveData data = SaveManager.Load(slotIndex);
            if (data != null)
            {
                fadeUI.Instance.levelTransicao.Transicao(data.currentScene);
            }
        }
        else
        {
            CreateNewSave();
        }
    }

    private void CreateNewSave()
    {
        SaveData newSave = new SaveData
        {
            playerCheckpoint = Vector2.zero,
            playerHealth = 3,
            currentScene = "Altior-Quarto",
            DashUnlocked = false,
            WalljumpUnlocked = false,
            attackUnlocked = false,
            powerUps = new List<PowerUps>(),
            XPlayer = 0,
            playTime = 0f
        };

        SaveManager.Save(newSave, slotIndex);
        GameManager.currentSaveSlot = slotIndex;
        fadeUI.Instance.levelTransicao.Transicao(newSave.currentScene);
        Debug.Log($"Novo save criado no slot {slotIndex}");
        StartCoroutine(SlotDelayUI());
    }

    private IEnumerator SlotDelayUI()
    {
        yield return new WaitForSeconds(1f);
        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        if (SaveManager.SaveExists(slotIndex))
        {
            SaveData data = SaveManager.Load(slotIndex);
            TimeSpan time = TimeSpan.FromSeconds(data.playTime);
            string tempo = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";

            slotItemConteudo.SetActive(true);
            slotItemVazio.SetActive(false);

            foreach (var img in imagensPoderes)
            {
                img.gameObject.SetActive(false);
            }

            if (data.currentScene == "Altior-Fuga" || data.currentScene == "Altior-PreFuga" || data.currentScene == "Altior-Quarto")
            {
                slotItemConteudo.GetComponentInChildren<Image>().sprite = imagensCenas[0];
            }
            else if (data.currentScene == "DimensaoTempo")
            {
                slotItemConteudo.GetComponentInChildren<Image>().sprite = imagensCenas[1];
            }
            else if (data.currentScene == "MontanhaIntro" || data.currentScene == "Boss&NPC")
            {
                slotItemConteudo.GetComponentInChildren<Image>().sprite = imagensCenas[2];
            }
            else if (data.currentScene == "CidadeCozinheiros")
            {
                slotItemConteudo.GetComponentInChildren<Image>().sprite = imagensCenas[3];
            }
            else if (data.currentScene == "ValeDeFerro")
            {
                slotItemConteudo.GetComponentInChildren<Image>().sprite = imagensCenas[4];
            }
            else if (data.currentScene == "SopeEletrico")
            {
                slotItemConteudo.GetComponentInChildren<Image>().sprite = imagensCenas[5];
            }
            else if (data.currentScene == "EloPerdido")
            {
                slotItemConteudo.GetComponentInChildren<Image>().sprite = imagensCenas[6];
            }

            slotItemConteudo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{data.currentScene}";
            slotItemConteudo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{tempo}";
            slotItemConteudo.transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = $"{data.playerHealth}";
            slotItemConteudo.transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = $"{data.XPlayer}";

            //A lista de powerUps.
            if (data.attackUnlocked)
            {
                data.powerUps.Add(PowerUps.Bastao);
            }
            foreach (var power in data.powerUps)
            {
                imagensPoderes[(int)power].gameObject.SetActive(true);
            }
        }
        else
        {
            slotItemVazio.SetActive(true);
            slotItemConteudo.SetActive(false);
        }
    }
}
