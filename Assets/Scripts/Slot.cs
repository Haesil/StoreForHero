using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IEndDragHandler , IPointerClickHandler
{
    // Inventory, Shelf, Market에서 사용하는 각 칸을 구현한 클래스
    // Selling 씬에서 사용하는 Shelf와 Inventory간의 이동은 드래그로 구현하고 Market 씬의 Market은 터치로 아이템을 구매하도록 구현.
    // 각 이동 시 개수를 묻는 창이 뜨고 그 창에서 상호작용해야 이동하도록 코루틴으로 구현.

    public int number;
    public Item item;
    public int count = 0;
    public bool flag = false;
    public bool isInventory = false;
    public bool isShelf = false;
    public bool isMarket = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // 슬롯을 드래그하는 상황을 구현
        if(GameManager.instance.curState != GameManager.State.market && !GameManager.instance.flag)
        {
            // 마켓 씬에 가있는 상태가 아니고, 판매중인 상태가 아닐 경우에만 드래그 가능.
            if (transform.parent.name == "Inventory")
            {
                // 드래그 되는 슬롯이 인벤토리의 슬롯인 경우
                isInventory = true;
                if (transform.childCount > 0)
                    transform.GetChild(0).SetParent(Inventory.instance.draggingItem.transform);
                Inventory.instance.draggingItem.transform.GetChild(0).position = Input.mousePosition;
            }
            if (transform.parent.name == "Shelf")
            {
                // 드래그 되는 슬롯이 Shelf의 슬롯 인경우
                isShelf = true;
                if (transform.childCount > 0)
                    transform.GetChild(0).SetParent(Shelf.instance.draggingItem.transform);
                Shelf.instance.draggingItem.transform.GetChild(0).position = Input.mousePosition;
            }
        }
        
        
    }
    public void OnPointerEnter(PointerEventData data)
    {
        // 터치포인터나 마우스가 슬롯에 들어올 경우 수행하는 내용
        if(transform.parent.name == "Market")
        {
            // Market UI의 슬롯인 경우 툴팁 변경
            if(Market.instance.type != Item.ItemType.Potion && item.itemPrice != 0)
                GameObject.Find("Inventory").transform.Find("ToolTip").GetComponent<Text>().text = item.itemName + "\n\n" + item.itemDescription + "\n\n 구매 가격 : " + (item.itemPrice * 9 / 10);
            else if(item.itemPrice != 0)
                GameObject.Find("Inventory").transform.Find("ToolTip").GetComponent<Text>().text = item.itemName + "\n\n" + item.itemDescription + "\n\n 제조 가격 : " + (item.itemPrice / 5) + "\n 제작 재료 : " + ItemList.instance.items[item.basement].itemName + " 2개";
        }
        else if (item.itemPrice != 0)   // 인벤토리 UI의 슬롯인 경우 툴팁 변경
            GameObject.Find("Inventory").transform.Find("ToolTip").GetComponent<Text>().text = item.itemName + "\n\n" + item.itemDescription + "\n\n 판매 가격 : " + item.itemPrice;
        if (GameManager.instance.curState != GameManager.State.market)
        {
            // Market 씬이 아닌 경우 각 UI의 enteredSlot을 자기 자신으로 함. (드래그 끝났을 시 enteredSlot과 draggedItem을 교환하기 위해서)
            Inventory.instance.enteredSlot = this;
            if (UImanager.instance.isShelf)
                Shelf.instance.enteredSlot = this;
        }
        
    }

    public void OnPointerExit(PointerEventData data)
    {
        if(GameManager.instance.curState != GameManager.State.market)
        {
            // 빠져나왔을 때 enteredSlot을 비움
            Inventory.instance.enteredSlot = null;
            if (UImanager.instance.isShelf)
                Shelf.instance.enteredSlot = null;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // Market씬에서 클릭시 수행하는 내용.
        if (transform.parent.name == "Market" && GameManager.instance.curState == GameManager.State.market)
        {
            flag = true;
            GameObject.Find("Market").transform.Find("Image").gameObject.SetActive(true);
            // 개수를 입력하는 함수가 주어질 때까지 기다림.
            if (Market.instance.type != Item.ItemType.Potion)
            {
                StartCoroutine(InputWait(2));
                GameObject.Find("Market").transform.Find("Image").Find("Text").GetComponent<Text>().text = "몇 개를 구입하시겠습니까?";
            }
            else
            {
                StartCoroutine(InputWait(3));
                GameObject.Find("Market").transform.Find("Image").Find("Text").GetComponent<Text>().text = "몇 개를 제조하시겠습니까?";
            }
            UImanager.instance.slot = this;
            UImanager.instance.UIflag = false;
        }
    }
    public void OnEndDrag(PointerEventData data)
    {
        // 드래그가 끝났을 때 수행하는 내용
        if (GameManager.instance.curState != GameManager.State.market && !GameManager.instance.flag)
        {
            if (isInventory)
            {
                // Inventory의 슬롯이 드래그되었던 경우
                isInventory = false;
                Inventory.instance.draggingItem.transform.GetChild(0).SetParent(transform);
                transform.GetChild(0).localPosition = Vector3.zero;
                if (Inventory.instance.enteredShelf != null && transform.parent.name != "Shelf")
                {
                    // 드래그하여 Shelf에 넣은 경우 몇 개를 이동할지 묻는 UI가 뜨고 그것에 응답해야 Inventory에서 빼고 Shelf에 추가함
                    UImanager.instance.itemCount = item.itemCount;
                    UImanager.instance.slot = this;
                    flag = true;
                    StartCoroutine(InputWait(0));
                    GameObject.Find("Shelf").transform.Find("Image").gameObject.SetActive(true);
                    UImanager.instance.UIflag = false;
                }
                else if (Inventory.instance.enteredSlot != null)
                {
                    // 같은 Inventory내의 Slot에서 드래그가 끝났을 경우 둘의 위치를 바꿈
                    Item tmpItem = item;
                    item = Inventory.instance.enteredSlot.item;
                    Inventory.instance.enteredSlot.item = tmpItem;
                    Inventory.instance.ItemImageChange(this);
                    Inventory.instance.ItemImageChange(Inventory.instance.enteredSlot);
                }
            }
            if (isShelf)
            {
                // Shelf의 슬롯이 드래그되었던 경우
                isShelf = false;
                Shelf.instance.draggingItem.transform.GetChild(0).SetParent(transform);
                transform.GetChild(0).localPosition = Vector3.zero;
                if (Shelf.instance.enteredInventory != null && transform.parent.name != "Inventory")
                {
                    // 드래그하여 Inventory에 넣은 경우 몇 개를 이동할지 묻는 UI가 뜨고 그것에 응답해야 Shelf에서 빼고 Inventory에 추가함
                    UImanager.instance.itemCount = GameManager.instance.shelves[Shelf.instance.shelfNum, item.itemID];
                    UImanager.instance.slot = this;
                    flag = true;
                    StartCoroutine(InputWait(1));
                    GameObject.Find("Inventory").transform.Find("Image").gameObject.SetActive(true);
                    UImanager.instance.UIflag = false;
                }
                else if (Shelf.instance.enteredSlot != null)
                {
                    // 같은 Shelf 내의 Slot에서 드래그가 끝났을 경우 둘의 위치를 바꿈
                    Item tmpItem = item;
                    item = Shelf.instance.enteredSlot.item;
                    Shelf.instance.enteredSlot.item = tmpItem;
                    Shelf.instance.ItemImageChange(this);
                    Shelf.instance.ItemImageChange(Shelf.instance.enteredSlot);
                }
            }
        }
    }
    IEnumerator InputWait(int i)
    {
        while (true)
        {
            // 개수를 묻는 UI에 응답했을 때 flag가 true가 되어 break됨
            if (!flag)
                break;
            yield return null;
        }
        if(i ==0)
        {
            if (count != 0)
            {
                // Shelf에서 Inventory로 이동할 때 개수를 묻는 UI에 응답했을 시 수행하는 부분.
                Shelf.instance.AddItem(item.itemID, count);
                Inventory.instance.RemoveItem(item.itemID,count);
                Inventory.instance.ItemImageChange(this);
                count = 0;
            }
        }
        else if(i==1)
        {
            if (count != 0)
            {
                // Inventory에서 Shelf로 이동할 때 개수를 묻는 UI에 응답했을 시 수행하는 부분.
                int check = Shelf.instance.RemoveItem(item.itemID, count);
                Inventory.instance.AddItem(item.itemID, count);
                
                if (check <= 0)
                {
                    item = new Item();
                }
                Shelf.instance.ItemImageChange(this);
                count = 0;
            }
        }
        else if(i==2)
        {
            if (count != 0)
            {
                // Market에서 구매할 때 개수를 묻는 UI에 응답했을 시 수행하는 부분
                Inventory.instance.AddItem(item.itemID, count);
                GameManager.instance.Gold -= (count * item.itemPrice*9/10);
                Inventory.instance.GoldUpdate();
                count = 0;
            }
        }
        else if(i==3)
        {
            if (count != 0)
            {
                // Market에서 연금술사를 통해 아이템을 제작할 경우 수행하는 부분
                Inventory.instance.AddItem(item.itemID, count);
                GameManager.instance.Gold -= (count * item.itemPrice/ 5);
                Inventory.instance.GoldUpdate();
                Inventory.instance.RemoveItem(item.basement, 2 * count);
                count = 0;
            }
        }
    }
}
