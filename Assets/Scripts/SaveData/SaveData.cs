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
    public List<PowerUpData> powerUps = new List<PowerUpData> { new PowerUpData(PowerUps.Bastao), new PowerUpData(PowerUps.Arco) };

    // Arma equipada atualmente por categoria
    public string equippedPrimary = "";
    public string equippedSecondary = "";

    // --- Estátuas ---
    public List<EstatuaData> estatuaDataList = new List<EstatuaData>();

    // --- Inventário ---
    public inventory_System.InventorySaveData inventoryData = new inventory_System.InventorySaveData();

    // --- Fragmentos ---
    public FragmentoSystem.FragmentoSaveData fragmentoData = new FragmentoSystem.FragmentoSaveData();

    // --- Quests ---
    public QuestManager.QuestSaveData questData = new QuestManager.QuestSaveData();

    // --- Infos ---
    public InfoManager.InfoSaveData infoData = new InfoManager.InfoSaveData();
}

public enum PowerUpCategory
{
    ArmaPrimaria,
    ArmaSecundaria
}

public enum PowerUps
{
    Bastao,
    Arco,
    Marreta,
    Sino,
    Mascara
}

public static class PowerUpsExtensions
{
    public static Dictionary<PowerUps, PowerUpCategory> powerUpCategories = new Dictionary<PowerUps, PowerUpCategory>
    {
        { PowerUps.Bastao, PowerUpCategory.ArmaPrimaria },
        { PowerUps.Marreta, PowerUpCategory.ArmaPrimaria },
        { PowerUps.Arco, PowerUpCategory.ArmaSecundaria },
        { PowerUps.Sino, PowerUpCategory.ArmaSecundaria },
        { PowerUps.Mascara, PowerUpCategory.ArmaSecundaria }
    };

    public static string GetStringCategory(this PowerUpCategory powerUpCCategory)
    {
        //Trasnformar o enum em string legível
        switch (powerUpCCategory)
        {
            case PowerUpCategory.ArmaPrimaria:
                return "Arma Primária";
            case PowerUpCategory.ArmaSecundaria:
                return "Arma Secundária";
            default:
                return powerUpCCategory.ToString();
        }
    }
}

[System.Serializable]
public class PowerUpData
{
    public PowerUps name;
    public PowerUpCategory category;

    public PowerUpData(PowerUps powerUp)
    {
        name = powerUp;
        category = PowerUpsExtensions.powerUpCategories[powerUp];
    }
}

public class HasPowerUp
{
    public static bool HasPowerUps(PowerUps powerUp)
    {
        return SaveData.Instance.powerUps.Exists(p => p.name == powerUp);
    }
}