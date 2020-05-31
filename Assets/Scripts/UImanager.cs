using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UImanager : MonoBehaviour
{
    // UI를 띄우고 끄는 함수들을 관리하는 클래스

    static public UImanager instance;
    public bool isInventory;
    public bool isShelf;
    public bool isMarket;
    public bool UIflag;                 // 아이템 이동 관련 UI가 떠있을때 UI가 꺼지지 않도록 확인하는 변수
    GameObject shelf;
    public int itemCount;
    public Slot slot;
    int count = 0;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        isInventory = false;
        isShelf = false;
        isMarket = false;
        UIflag = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.curState == GameManager.State.selling)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Selling 씬에서 터치 시 수행하는 내용 
                // Inventory나 Shelf가 켜져있거나 UI 터치시에는 작동하지 않음
                if (!(isInventory || isShelf) && !EventSystem.current.IsPointerOverGameObject())
                {
                    // 마우스의 Position으로 Ray발사
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(ray.origin, ray.direction * 19, out hitInfo, 100.0f, (1 << 10)))
                    {
                        //Ray가 충돌한 오브젝트가 Reactable Layer인 경우
                        InitShelf();
                        switch (hitInfo.transform.name)
                        {
                            case "shelf1":
                                shelf.GetComponent<Shelf>().shelfSet(0);
                                ShelfOn();
                                break;
                            case "shelf2":
                                shelf.GetComponent<Shelf>().shelfSet(1);
                                ShelfOn();
                                break;
                            case "shelf3":
                                shelf.GetComponent<Shelf>().shelfSet(2);
                                ShelfOn();
                                break;
                            case "shelf4":
                                shelf.GetComponent<Shelf>().shelfSet(3);
                                ShelfOn();
                                break;
                            case "goMarket":
                                // 판매중이 아니면 Market 씬으로 이동
                                if (!GameManager.instance.flag)
                                {
                                    LoadScene();
                                }
                                break;
                        }
                    }
                }
            }
        }
        else if (GameManager.instance.curState == GameManager.State.market)
        {
            if (!(isInventory || isMarket) && Input.GetMouseButtonDown(0))
            {
                // Market 씬에서 다른 UI가 켜져있지 않은 경우
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray.origin, ray.direction * 19, out hitInfo, 100.0f, (1 << 11)))
                {
                    //Ray가 충돌한 오브젝트가 NPC Layer인 경우
                    switch (hitInfo.transform.name)
                    {
                        case "HerbSeller":
                            GameObject.Find("Main Camera").GetComponent<CameraMove>().SetTarget(1);
                            MarketOnOff(0);
                            break;
                        case "grocery":
                            GameObject.Find("Main Camera").GetComponent<CameraMove>().SetTarget(2);
                            MarketOnOff(1);
                            break;
                        case "alchemist":
                            GameObject.Find("Main Camera").GetComponent<CameraMove>().SetTarget(3);
                            MarketOnOff(2);
                            break;
                    }
                }
            }
        }
    }
    
    void ShelfOn()
    {
        // Shelf UI를 켜는 함수
        shelf.SetActive(true);
        isShelf = true;
        InventoryOnOff();
    }

    public void InventoryOnOff()
    {
        if(UIflag)
        {
            // 아이템 이동 관련 UI가 떠있을때 UI가 꺼지지 않도록 확인
            if (isInventory)
            {
                if (isShelf)
                {
                    // 인벤토리 끌때 Shelf가 켜져있으면 같이 종료
                    GameObject.Find("Canvas").transform.Find("Shelf").gameObject.GetComponent<Shelf>().quitShelf();
                    GameObject.Find("Canvas").transform.Find("Shelf").gameObject.SetActive(false);
                    isShelf = false;
                }
                if(isMarket)
                {
                    // 인벤토리 끌때 Market이 켜져있으면 같이 종료
                    GameObject.Find("Canvas").transform.Find("Market").gameObject.SetActive(false);
                    isMarket = false;
                }
                GameObject.Find("Canvas").transform.Find("Inventory").gameObject.SetActive(false);
                isInventory = false;
            }
            else
            {
                GameObject.Find("Canvas").transform.Find("Inventory").Find("Gold").Find("Text").gameObject.GetComponent<Text>().text = "" + GameManager.instance.Gold;
                GameObject.Find("Canvas").transform.Find("Inventory").Find("ToolTip").gameObject.GetComponent<Text>().text = "";
                GameObject.Find("Canvas").transform.Find("Inventory").gameObject.SetActive(true);
                isInventory = true;
                Inventory.instance.flag = true;
            }
        }
    }

    public void InitShelf()
    {
        // shelf object를 미리 지정해둠
        shelf = GameObject.Find("Canvas").transform.Find("Shelf").gameObject;
    }

    public void ShelfOff()
    {
        if(UIflag)
        {
            // 아이템 이동 관련 UI가 떠있을때 UI가 꺼지지 않도록 확인
            if (isShelf)
            {
                GameObject.Find("Canvas").transform.Find("Shelf").gameObject.GetComponent<Shelf>().quitShelf();
                GameObject.Find("Canvas").transform.Find("Shelf").gameObject.SetActive(false);
                isShelf = false;
            }
            if (isInventory)
            {
                GameObject.Find("Canvas").transform.Find("Inventory").gameObject.SetActive(false);
                isInventory = false;
            }
        }
    }

    public void MarketOnOff(int num)
    {
        if (isMarket)
        {
            if (isInventory)
            {
                GameObject.Find("Canvas").transform.Find("Inventory").gameObject.SetActive(false);
                isInventory = false;
            }
            Market.instance.quitMarket();
            GameObject.Find("Canvas").transform.Find("Dialogue").Find("Text").GetComponent<Text>().text = "";
            GameObject.Find("Canvas").transform.Find("Dialogue").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("Market").gameObject.SetActive(false);
            GameObject.Find("Main Camera").GetComponent<CameraMove>().SetTarget(0);
            isMarket = false;
        }
        else
        {
            if (!isInventory)
            {
                // Market 켤 때 인벤토리가 켜져있지 않으면 같이 켬
                GameObject.Find("Canvas").transform.Find("Inventory").gameObject.SetActive(true);
                Inventory.instance.transform.Find("Gold").Find("Text").gameObject.GetComponent<Text>().text = "" + GameManager.instance.Gold;
                isInventory = true;
                Inventory.instance.flag = true;
            }
            GameObject.Find("Canvas").transform.Find("Dialogue").gameObject.SetActive(true);
            GameManager.instance.NPC = num;
            GameManager.instance.isParse = true;
            GameObject.Find("Canvas").transform.Find("Market").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("Market").gameObject.GetComponent<Market>().SetType(num);
            GameObject.Find("Canvas").transform.Find("Market").gameObject.GetComponent<Market>().InitItem();
            isMarket = true;
        }
    }

    public void SellStart()
    {
        // 판매 시작 버튼을 누르면 수행하는 함수
        if (GameManager.instance.curState != GameManager.State.tutorial)
            GameManager.instance.StartSelling();
    }
    
    public void LoadScene()
    {
        GameManager.instance.LoadScene();
    }

    #region CountChange
    // 아이템 이동 및 구매 시 UI에서 Count를 설정하는 함수들
    public void plus()
    {
        count++;
        if(isMarket)
        {
            if (Market.instance.type == Item.ItemType.Potion)
            {
                if (count * 2 > ItemList.instance.items[slot.item.basement].itemCount || GameManager.instance.Gold < (slot.item.itemPrice / 5) * count)
                {
                    count--;
                }
            }
            else
            {
                if (GameManager.instance.Gold < slot.item.itemPrice * count * 9 / 10)
                {
                    count--;
                }
            }
            GameObject.Find("Canvas").transform.Find("Market").Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
        }
        else if(isShelf)
        {
            if (count > itemCount)
                count = itemCount;
            GameObject.Find("Canvas").transform.Find("Shelf").Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
            GameObject.Find("Canvas").transform.Find("Inventory").Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
        }
    }

    public void minus()
    {
        count--;
        if (count < 0)
            count = 0;
        if (isMarket)
        {
            GameObject.Find("Canvas").transform.Find("Market").Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
        }
        else if(isShelf)
        {
            GameObject.Find("Canvas").transform.Find("Shelf").Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
            GameObject.Find("Canvas").transform.Find("Inventory").Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
        }
    }

    public void plus10()
    {
        count += 10;
        if (isMarket)
        {
            if(Market.instance.type == Item.ItemType.Potion)
            {
                while(count*2 > ItemList.instance.items[slot.item.basement].itemCount || GameManager.instance.Gold < (slot.item.itemPrice/5) * count)
                {
                    count--;
                }
            }
            else
            {
                while (GameManager.instance.Gold < slot.item.itemPrice * count * 9 / 10)
                {
                    count--;
                }
            }
            GameObject.Find("Canvas").transform.Find("Market").Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
        }
        else if(isShelf)
        {
            if (count > itemCount)
                count = itemCount;
            GameObject.Find("Canvas").transform.Find("Shelf").Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
            GameObject.Find("Canvas").transform.Find("Inventory").Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
        }
    }

    public void minus10()
    {
        count -= 10;
        if (count < 0)
            count = 0;
        if(isMarket)
        {
            GameObject.Find("Canvas").transform.Find("Market").Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
        }
        else if(isShelf)
        {
            GameObject.Find("Canvas").transform.Find("Shelf").Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
            GameObject.Find("Canvas").transform.Find("Inventory").Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
        }
    }
    #endregion

    #region ItemSetting
    // 아이템 이동 시 확인 버튼을 눌렀을 때 수행하는 함수들
    public void setProduct()
    {
        slot.count = count;
        slot.flag = false;
        count = 0;
        Shelf.instance.transform.Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
        Inventory.instance.transform.Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
        Shelf.instance.transform.Find("Image").gameObject.SetActive(false);
        UIflag = true;
    }

    public void getProduct()
    {
        slot.count = count;
        slot.flag = false;
        count = 0;
        Shelf.instance.transform.Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
        Inventory.instance.transform.Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
        Inventory.instance.transform.Find("Image").gameObject.SetActive(false);
        UIflag = true;
    }

    public void buyProduct()
    {
        slot.count = count;
        slot.flag = false;
        count = 0;
        Market.instance.transform.Find("Image").Find("Count").GetComponent<Text>().text = "" + count;
        Market.instance.transform.Find("Image").gameObject.SetActive(false);
        UIflag = true;
    }
    #endregion
    
    // 결과창 On Off
    public void ResultUIOn()
    {
        GameObject.Find("Canvas").transform.Find("Result").gameObject.SetActive(true);
    }
    public void ResultUIOff()
    {
        GameObject.Find("Canvas").transform.Find("Result").gameObject.SetActive(false);
    }

    // 종료 UI On Off
    public void QuitUIOn()
    {
        Time.timeScale = 0;
        GameObject.Find("Canvas").transform.Find("QuitUI").gameObject.SetActive(true);
    }

    public void QuitUIOff()
    {
        Time.timeScale = 1;
        GameObject.Find("Canvas").transform.Find("QuitUI").gameObject.SetActive(false);
    }

    // 게임 종료
    public void GameQuit()
    {
        GameManager.instance.GameQuit();
    }
}

