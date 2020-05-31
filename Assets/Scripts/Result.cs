using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    // 하루 판매량을 보여주는 UI를 만드는 클래스
    // GameManager의 result 배열을 이용하여 결과창을 띄움.

    ItemList list;
    public GameObject slot;
    public List<GameObject> slotList;
    int gold;
    bool flag = true;
    // Start is called before the first frame update
    void Start()
    {
        gold = 0;
        list = ItemList.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (flag)
        {
            flag = false;
            InitResult();
        }
            
    }

    public void InitResult()
    {
        int index = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                while (index < ItemList.instance.limit && GameManager.instance.result[index] == 0)
                    index++;
                if (index >= ItemList.instance.limit)
                    break;
                GameObject newSlot = Instantiate(slot);
                newSlot.name = "Slot" + (i + 1) + "." + (j + 1);
                newSlot.transform.SetParent(this.transform);
                RectTransform slotRect = newSlot.GetComponent<RectTransform>();
                int width = Screen.width;
                slotRect.anchoredPosition = new Vector3(-220 + (j * 110), 250 - (i * 110), 0);
                slotRect.sizeDelta = new Vector2(80 * Screen.width / 1920, 80 * Screen.width / 1920);
                newSlot.transform.Find("Image").gameObject.SetActive(true);
                newSlot.transform.Find("Image").GetComponent<Image>().sprite = list.items[index].itemIcon;
                newSlot.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(75 * width / 1920, 75 * width / 1920);
                newSlot.transform.Find("Count").GetComponent<RectTransform>().sizeDelta = new Vector2(20 * width / 1920, 20 * width / 1920);
                newSlot.transform.Find("Count").GetComponent<Text>().text = "" + GameManager.instance.result[index];
                slotList.Add(newSlot);
                gold += list.items[index].itemPrice* GameManager.instance.result[index];
                index++;
            }
            if (index >= ItemList.instance.limit)
                break;
        }
        transform.Find("Gold").Find("Text").GetComponent<Text>().text = "" + gold;
        int num = GameManager.instance.ExpUP(gold);
        if (num != 0)
        {
            transform.Find("Level").gameObject.SetActive(true);
            transform.Find("Level").Find("Original").GetComponent<Text>().text = "" + (GameManager.instance.level - num);
            transform.Find("Level").Find("Changed").GetComponent<Text>().text = "" + GameManager.instance.level;
            if(ItemList.instance.flag)
            {
                ItemList.instance.flag = false;
                transform.Find("Level").Find("Text").GetComponent<Text>().text = "새로운 상품을 얻을 수 있습니다!";
            }
        }
    }

    public void CloseResult()
    {
        for(int i = 0; i < slotList.Count; i++)
        {
            Destroy(slotList[i]);
        }
        
        gold = 0;
        slotList.Clear();
        flag = true;
        GameManager.instance.SaveFile();
    }
}
