using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DocumentosType
{
    Robert,
    Inventigacao,
    Game
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Tipos")]
public class Item_Types : ItemData
{
    public DocumentosType documentosType;
}
