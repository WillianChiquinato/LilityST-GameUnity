using System;

[Serializable]
public class FragmentoItem
{
    public FragmentoData FragmentoData;
    public int stackSize;

    public FragmentoItem(FragmentoData fragmentoData)
    {
        FragmentoData = fragmentoData;
        AddStack();
    }

    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;
    public void ResetStack() => stackSize = 0;
}
