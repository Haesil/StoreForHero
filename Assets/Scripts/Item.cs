using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Item
{
    // 아이템을 구현한 클래스

    public int itemID;
    public string itemName;
    public int itemPrice;
    public string itemDescription;
    public Sprite itemIcon;
    public int itemCount;
    public ItemType itemType;
    public int basement;            // 아이템 조합시 필요한 재료의 ID

    public enum ItemType
    {
        Herb,
        Potion,
        DryFood
    }

    public Item()
    {

    }

    public Item(string img, int ID, string name, int price, string des, ItemType type, int count)
    {
        itemID = ID;
        itemName = name;
        itemPrice = price;
        itemDescription = des;
        itemType = type;
        itemCount = count;
        itemIcon = Resources.Load<Sprite>("ItemImages/" + img);
    }
    public Item(string img, int ID, string name, int price, string des, ItemType type, int count, int b)
    {
        itemID = ID;
        itemName = name;
        itemPrice = price;
        itemDescription = des;
        itemType = type;
        itemCount = count;
        itemIcon = Resources.Load<Sprite>("ItemImages/" + img);
        basement = b;
    }
}
