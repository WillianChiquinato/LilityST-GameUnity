using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData instance;
    public static SaveData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SaveData();
            }
            return instance;
        }
        set { instance = value; }
    }

    public float playTime;
    public int XPlayer = 0;
    public Vector2 playerCheckpoint;
    public int playerHealth;
    public string currentScene;
    public bool DashUnlocked = false;
    public bool WalljumpUnlocked = false;
    public List<PowerUps> powerUps = new List<PowerUps> { PowerUps.Arco };

    // --- Inventário ---
    public inventory_System.InventorySaveData inventoryData = new inventory_System.InventorySaveData();

    // --- Fragmentos ---
    public FragmentoSystem.FragmentoSaveData fragmentoData = new FragmentoSystem.FragmentoSaveData();

    // --- Quests ---
    public QuestManager.QuestSaveData questData = new QuestManager.QuestSaveData();

    // --- Infos ---
    public InfoManager.InfoSaveData infoData = new InfoManager.InfoSaveData();
}

public enum PowerUps
{
    Bastao,
    Arco,
    Marreta,
    Sino,
    Mascara
}