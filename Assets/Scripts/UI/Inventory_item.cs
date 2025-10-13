using System;
using System.Collections.Generic;

[Serializable]
public class Inventory_item
{
    public ItemData itemData;
    public int stackSize;

    public Inventory_item(ItemData _newItem)
    {
        itemData = _newItem;
        AddStack();
    }

    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;
    public void ResetStack() => stackSize = 0;

    public static implicit operator List<object>(Inventory_item v)
    {
        throw new NotImplementedException();
    }
}
