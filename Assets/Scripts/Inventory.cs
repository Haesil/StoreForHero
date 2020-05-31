using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Inventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 인벤토리를 구현한 클래스
    // 인벤토리에 아이템을 추가하고 제거하는 기능을 구현함.
    // 중복된 아이템을 얻었을 때 같은 칸에 넣어주기 위하여 HashSet을 사용함.

    public static Inventory instance;
    public GameObject slot;                             // 인벤토리의 칸을 구현한 Object
    public GameObject draggingItem;                     // 인벤토리의 child중 드래그되고 있는 슬롯이 저장되는 곳.
    public List<Slot> slotScripts = new List<Slot>();
    public Slot enteredSlot;                            // 터치되거나 마우스가 들어간 슬롯
    public Shelf enteredShelf;                          // 터치되거나 마우스가 들어간 Shelf(UI)
    public bool flag;                                   // 인벤토리를 열 때 인벤토리 내용을 최신화 하게 할때 true
    
    private ItemList list;
    private HashSet<int> set;

    #region drag
    // Shelf에서 인벤토리로 드래그 할 때 필요한 부분
    public void OnPointerEnter(PointerEventData data)
    {
        if(UImanager.instance.isShelf)
            Shelf.instance.enteredInventory = this;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (UImanager.instance.isShelf)
            Shelf.instance.enteredInventory = null;
    }
    #endregion

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GameObject newSlot = Instantiate(slot);
                newSlot.name = "Slot" + (i + 1) + "." + (j + 1);
                newSlot.transform.SetParent(transform);
                RectTransform slotRect = newSlot.GetComponent<RectTransform>();
                slotRect.anchoredPosition = new Vector3(-220 + (j * 110), 320 - (i * 110), 0);
                int width = Screen.width;
                // 해상도에 따라 슬롯 크기를 변화시킴.
                slotRect.localScale = new Vector3(width / 1920, width / 1920, 1);
                newSlot.transform.Find("Image").gameObject.SetActive(false);
                newSlot.GetComponent<Slot>().number = i * 4 + j;
                slotScripts.Add(newSlot.GetComponent<Slot>());
            }
        }
        list = ItemList.instance;
        set = new HashSet<int>();
        flag = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (flag)
        {
            flag = false;
            for (int i = 0; i < slotScripts.Count; i++)
            {
                // 게임 매니저의 inventory 변수를 이용하여 인벤토리 최신화
                if(GameManager.instance.inventory[i] != -1)
                {
                    // 인벤토리가 채워진 경우
                    set.Add(GameManager.instance.inventory[i]);
                    slotScripts[i].item = list.items[GameManager.instance.inventory[i]];
                    slotScripts[i].transform.Find("Image").gameObject.SetActive(true);
                    slotScripts[i].transform.Find("Image").GetComponent<Image>().sprite = slotScripts[i].GetComponent<Slot>().item.itemIcon;
                    slotScripts[i].transform.Find("Image").Find("Count").Find("Text").GetComponent<Text>().text = "" + slotScripts[i].item.itemCount;
                }
                else
                {
                    // 인벤토리가 빈 경우
                    slotScripts[i].item = new Item();
                    slotScripts[i].transform.Find("Image").gameObject.SetActive(false);
                    slotScripts[i].transform.Find("Image").GetComponent<Image>().sprite = null;
                    slotScripts[i].transform.Find("Image").Find("Count").Find("Text").GetComponent<Text>().text = "";
                }
            }
            
        }
    }

    public void AddItem(int number, int count)
    {
        if(set.Add(number))
        {
            // 인벤토리에 중복된 아이템이 없는 경우
            for (int i = 0; i < slotScripts.Count; i++)
            {
                if (slotScripts[i].item.itemPrice == 0)
                {
                    slotScripts[i].item = list.items[number];
                    slotScripts[i].item.itemCount = count;
                    slotScripts[i].transform.Find("Image").gameObject.SetActive(true);
                    slotScripts[i].transform.Find("Image").GetComponent<Image>().sprite = slotScripts[i].GetComponent<Slot>().item.itemIcon;
                    slotScripts[i].transform.Find("Image").Find("Count").Find("Text").GetComponent<Text>().text = "" + slotScripts[i].item.itemCount;
                    GameManager.instance.inventory[i] = number;
                    break;
                }
            }
        }
        else
        {
            // 인벤토리에 중복된 아이템이 있는 경우
            for (int i = 0; i < slotScripts.Count; i++)
            {
                if (slotScripts[i].item.itemID == number)
                {
                    slotScripts[i].item.itemCount += count;
                    slotScripts[i].transform.Find("Image").Find("Count").Find("Text").GetComponent<Text>().text = "" + slotScripts[i].item.itemCount;
                    break;
                }
            }
        }
    }
    

    public void RemoveItem(int number, int count)
    {
        for (int i = 0; i < slotScripts.Count; i++)
        {
            if (slotScripts[i].item.itemID == number)
            {
                if (slotScripts[i].item.itemCount == count)
                {
                    // 아이템을 다 빼서 슬롯이 빈경우
                    slotScripts[i].item = new Item();
                    set.Remove(number);
                    slotScripts[i].item.itemCount -= count;
                    GameManager.instance.inventory[i] = -1;
                }
                else
                {
                    // 아이템이 남은 경우
                    slotScripts[i].item.itemCount -= count;
                    slotScripts[i].transform.Find("Image").Find("Count").Find("Text").GetComponent<Text>().text = "" + slotScripts[i].item.itemCount;
                }
                ItemImageChange(slotScripts[i]);
                break;
            }
        }
    }

    public void ItemImageChange(Slot s)
    {
        // 아이템의 변화 후 슬롯을 최신화 할때
        if (s.item.itemPrice == 0)
            s.transform.GetChild(0).gameObject.SetActive(false);
        else
        {
            s.transform.GetChild(0).gameObject.SetActive(true);
            s.transform.GetChild(0).GetComponent<Image>().sprite = s.item.itemIcon;
            s.transform.Find("Image").Find("Count").Find("Text").GetComponent<Text>().text = "" + s.item.itemCount;
        }

    }

    public void GoldUpdate()
    {
        // 골드를 최신화 할 때
        transform.Find("Gold").Find("Text").gameObject.GetComponent<Text>().text = "" + GameManager.instance.Gold;
    }
}
