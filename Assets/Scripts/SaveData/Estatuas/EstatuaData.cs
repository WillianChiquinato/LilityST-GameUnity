using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EstatuaData
{
    public string estatuaID;
    public Vector3 position;
    public int storedXP;
    public List<ItemData> droppedItems;
}
