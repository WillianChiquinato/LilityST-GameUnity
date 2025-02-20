using System;

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
}
