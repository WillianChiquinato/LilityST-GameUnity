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

    public Vector2 playerCheckpoint;
    public int playerHealth;
    public string currentScene;
    public bool CameraCorrected = true;
    public bool DashUnlocked = true;
    public bool WalljumpUnlocked = true;
    public bool JumpUnlocked = true;
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