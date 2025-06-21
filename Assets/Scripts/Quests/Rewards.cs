using System.Collections.Generic;

[System.Serializable]
public class Rewards
{
    [System.Flags]
    public enum RewardType
    {
        None = 0,
        XP = 1 << 0,
        Item = 1 << 1
    }

    public RewardType TipoRecompensa;

    public int xpPlayer;
    public List<ItemData> itensData;
}