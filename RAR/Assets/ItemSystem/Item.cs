
using System;
using UnityEngine;

[Serializable]
public class Item//物品类
{
    public string itemID;//物品ID
    public int itemCount;//物品数量

    [NonSerialized]
    public ItemSO itemSO;//物品数据脚本

    public Item(string itemID)//物品类构造函数(物品ID)
    {
        this.itemID = itemID;
        this.itemCount = 1;
        Init();
    }
    public Item(string itemID, int itemCount)//物品类构造函数(物品ID,物品数量)
    {
        this.itemID = itemID;
        this.itemCount = itemCount;
        Init();
    }
    public void Init()
    {
        GetItemSO();
    }
    public void GetItemSO()
    {
        ItemSO _itemSO = Resources.Load<ItemSO>("ItemSO/" + itemID);
        if (_itemSO != null)
        {
            this.itemSO = _itemSO;
        }
    }

}
