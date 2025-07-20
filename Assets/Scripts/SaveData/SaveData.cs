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
    }

    public int XPlayer = 0;
    public Vector2 playerCheckpoint;
    public int playerHealth;
    public string currentScene;
    public bool DashUnlocked = true;
    public bool WalljumpUnlocked = true;
    public bool attackUnlocked = true;
    public List<PowerUps> powerUps = new List<PowerUps> { PowerUps.Arco };
}

public enum PowerUps
{
    Bastao,
    Arco,
    Marreta,
    Sino,
    Mascara
}