using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Shelf : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 상품을 원하는 Shelf에 옮길 때 Shelf의 상태를 보여주는 UI 클래스
    // Shelf에 아이템을 추가하고 제거하는 기능 구현
    // 같은 Shelf에 같은 종류의 아이템을 넣을 경우를 처리하기 위해 HashSet사용

    public static Shelf instance;
    ItemList list;
    public GameObject slot;                     // Shelf의 칸을 구현한 오브젝트(Slot 스크립트를 가지고 있음)
    public List<Slot> slotList;
    public int shelfNum;
    bool flag = true;
    HashSet<int> set;
    public GameObject draggingItem;             // Shelf의 child중 드래그 되고 있는 슬롯이 저장되는 곳
    public Slot enteredSlot;                    // 터치되거나 마우스가 들어간 슬롯
    public Inventory enteredInventory;          // 터치되거나 마우스가 들어간 인벤토리(UI)

    #region drag
    // 인벤토리에서 Shelf로 드래그 할 때 필요한 부분
    public void OnPointerEnter(PointerEventData data)
    {
        Inventory.instance.enteredShelf = this;
    }

    public void OnPointerExit(PointerEventData data)
    {
        Inventory.instance.enteredShelf = null;
    }
    #endregion

    // Start is called before the first frame update
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
                slotRect.anchoredPosition = new Vector3(-220 + (j * 110), 320 - (i * 110), 0);
                // 해상도에 따라 슬롯 크기를 변화시킴.
                slotRect.localScale = new Vector3(width / 1920, width / 1920, 1);
                newSlot.transform.Find("Image").gameObject.SetActive(false);
                newSlot.GetComponent<Slot>().number = i * 4 + j;
                slotList.Add(newSlot.GetComponent<Slot>());
            }
        }
        list = ItemList.instance;
        set = new HashSet<int>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flag)
        {
            flag = false;
            int index = 0;
            
            for (int i = 0; i < ItemList.instance.limit; i++)
            {
                // 게임 매니저의 Shelves 변수를 이용하여 인벤토리 최신화
                if (GameManager.instance.shelves[shelfNum, i] != 0)
                {
                    set.Add(i);
                    slotList[index].item = list.items[i];
                    slotList[index].transform.Find("Image").Find("Count").Find("Text").GetComponent<Text>().text = "" + GameManager.instance.shelves[shelfNum, i];
                    slotList[index].transform.Find("Image").gameObject.SetActive(true);
                    slotList[index].transform.Find("Image").GetComponent<Image>().sprite = slotList[index].GetComponent<Slot>().item.itemIcon;
                    slotList[index].transform.Find("Image").Find("Count").Find("Text").GetComponent<Text>().text = "" + GameManager.instance.shelves[shelfNum, i];
                    index++;
                }
            }

        }

        
    }
    public void AddItem(int number, int count)
    {
        // Shelf에 아이템을 추가하는 함수
        GameManager.instance.shelves[shelfNum,number] += count;
        if(set.Add(number))
        {
            // Shelf에 같은 종류의 아이템이 없는 경우
            for (int i = 0; i < slotList.Count; i++)
            {
                if (slotList[i].item.itemPrice == 0)
                {
                    slotList[i].item = list.items[number];
                    slotList[i].transform.Find("Image").gameObject.SetActive(true);
                    slotList[i].transform.Find("Image").GetComponent<Image>().sprite = slotList[i].GetComponent<Slot>().item.itemIcon;
                    slotList[i].transform.Find("Image").Find("Count").Find("Text").GetComponent<Text>().text = "" + GameManager.instance.shelves[shelfNum, number];
                    break;
                }
            }
        }
        else
        {
            // Shelf에 같은 종류의 아이템이 있는 경우
            for (int i = 0; i < slotList.Count; i++)
            {
                if (slotList[i].item.itemID == number)
                {
                    slotList[i].transform.Find("Image").Find("Count").Find("Text").GetComponent<Text>().text = "" + GameManager.instance.shelves[shelfNum, number];
                }
            }
        }
    }

    public int RemoveItem(int number, int count)
    {
        // 아이템 제거하는 함수
        if (GameManager.instance.shelves[shelfNum, number] > count)
            GameManager.instance.shelves[shelfNum, number] -= count;
        else if (GameManager.instance.shelves[shelfNum, number] == count)
        {
            // 아이템을 전부 다 뺄 경우
            GameManager.instance.shelves[shelfNum, number] = 0;
            set.Remove(number);
        }
        // 제거한 슬롯에 count를 바꿔주기 위해 현재 아이템 갯수 return
        return GameManager.instance.shelves[shelfNum, number];
    }

    public void BuyItem(int num, int number, int count)
    {
        // 손님 캐릭터가 아이템을 구매하는 함수
        // 효과음 재생 후 아이템 갯수 줄이고 골드를 올림
        
        GameManager.instance.soundEffect.clip = Resources.Load<AudioClip>("SoundEffect/cash-register-sound-effect");
        GameManager.instance.soundEffect.Play();
        if (GameManager.instance.shelves[num, number] > count)
        {
            GameManager.instance.shelves[num, number] -= count;
        }
        else if (GameManager.instance.shelves[num, number] <= count)
        {
            GameManager.instance.shelves[num, number] = 0;
            set.Remove(number);
        }
    }

    public void shelfSet(int num)
    {
        // Shelf를 터치하여 Shelf UI를 켤때 초기화를 위한 변수 설정
        flag = true;
        shelfNum = num;
    }

    public void quitShelf()
    {
        // Shelf 닫을 때 Shlef 0으로 초기화
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].item = new Item();
            slotList[i].transform.Find("Image").gameObject.SetActive(false);
            slotList[i].transform.Find("Image").GetComponent<Image>().sprite = null;
            slotList[i].transform.Find("Image").Find("Count").Find("Text").GetComponent<Text>().text = "";

        }
        set.Clear();
    }

    public void ItemImageChange(Slot s)
    {

        if (s.item.itemPrice == 0)
            s.transform.GetChild(0).gameObject.SetActive(false);
        else
        {
            s.transform.GetChild(0).gameObject.SetActive(true);
            s.transform.GetChild(0).GetComponent<Image>().sprite = s.item.itemIcon;
            s.transform.Find("Image").Find("Count").Find("Text").GetComponent<Text>().text = "" + GameManager.instance.shelves[shelfNum,s.item.itemID];
        }

    }
}
