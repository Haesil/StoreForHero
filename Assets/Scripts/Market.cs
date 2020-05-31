using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Market : MonoBehaviour
{
    // Market 씬에서 NPC가 파는 아이템을 보여주는 클래스
    // 인벤토리를 응용하여 구현.

    public static Market instance;
    public GameObject slot;
    public List<Slot> slotList;
    ItemList list;
    
    public Item.ItemType type;  // 판매하는 사람이 판매할 아이템 타입

    void Awake()
    {
        instance = this;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GameObject newSlot = Instantiate(slot);
                newSlot.name = "Slot" + (i + 1) + "." + (j + 1);
                newSlot.transform.SetParent(this.transform);
                RectTransform slotRect = newSlot.GetComponent<RectTransform>();
                int width = Screen.width;
                slotRect.anchoredPosition = new Vector3(-220 + (j * 110), 210 - (i * 110), 0);
                // 해상도에 따라 슬롯 크기를 변화시킴.
                slotRect.localScale = new Vector3(width / 1920, width / 1920, 1);
                newSlot.transform.Find("Image").gameObject.SetActive(false);
                newSlot.GetComponent<Slot>().number = i * 4 + j;
                slotList.Add(newSlot.GetComponent<Slot>());
            }
        }
        list = ItemList.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetType(int num)
    {
        // UI에 띄울 아이템 타입 세팅
        switch(num)
        {
            case 0:
                type = Item.ItemType.Herb;
                break;
            case 1:
                type = Item.ItemType.DryFood;
                break;
            case 2:
                type = Item.ItemType.Potion;
                break;
        }
    }

    public void InitItem()
    {
        // 타입에 맞는 아이템을 UI에 띄움

        int index = 0;
        for (int i = 0; i < slotList.Count; i++)
        {
            while (index < list.limit && list.items[index].itemType != type)
            {
                index++;
            }
            if (index >= list.limit)
                break;
            if (slotList[i].item.itemPrice == 0)
            {
                slotList[i].item = list.items[index];
                slotList[i].transform.Find("Image").gameObject.SetActive(true);
                slotList[i].transform.Find("Image").GetComponent<Image>().sprite = slotList[i].GetComponent<Slot>().item.itemIcon;
                slotList[i].transform.Find("Image").Find("Count").gameObject.SetActive(false);
                index++;
            }
        }
    }

    public void quitMarket()
    {
        // UI닫을때 내용 초기화
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].item = new Item();
            slotList[i].transform.Find("Image").gameObject.SetActive(false);
            slotList[i].transform.Find("Image").GetComponent<Image>().sprite = null;
            slotList[i].transform.Find("Image").Find("Count").Find("Text").GetComponent<Text>().text = "";

        }
    }
}
