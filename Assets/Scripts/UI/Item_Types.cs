using UnityEngine;

public enum DocumentosType
{
    Robert,
    Investigacao,
    Game
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Tipos")]
public class Item_Types : ItemData
{
    public DocumentosType documentosType;
}
