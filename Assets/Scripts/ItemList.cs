using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    // 전체 아이템의 목록
    public static ItemList instance;
    public List<Item> items;
    public int limit;       // 레벨에 따른 아이템 제한
    public bool flag;       // 레벨에 따른 아이템 제한이 풀린 때에만 true, Result 창에 해금 안내 표시
    
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            limit = 8;
            flag = false;
            items = new List<Item>();
            items.Add(new Item("00", 0, "빨간 약초", 20, "흔히 발견되는 약초를 말린 것. \n빨간 약의 재료. 상처를 치료할 수 있다. \n정제없이 사용하면 효능이 약하다.", Item.ItemType.Herb, 0));
            items.Add(new Item("01", 1, "초록 약초", 40, "복통에 먹는 약초. \n복통 약의 재료. 정제없이 사용하면 효능이 미약하다.", Item.ItemType.Herb, 0));
            items.Add(new Item("02", 2, "파란 약초", 50, "해독제의 재료. \n바로 먹으면 효능이 없다.", Item.ItemType.Herb, 0));
            items.Add(new Item("03", 3, "빨간 약", 200, "상처에 바르는 약. \n생채기에 즉효약.", Item.ItemType.Potion, 0, 0));
            items.Add(new Item("04", 4, "복통 약", 400, "복통의 즉효약. \n의외로 중요함", Item.ItemType.Potion, 0, 1));
            items.Add(new Item("05", 5, "해독제", 500, "해독제. \n간단한 독을 해제한다.", Item.ItemType.Potion, 0, 2));
            items.Add(new Item("06", 6, "건빵", 250, "배고픈 모험가들의 친구. 물이 필수이다.", Item.ItemType.DryFood, 0));
            items.Add(new Item("07", 7, "말린 육포", 400, "육포. \n여행할 때의 필수품. 없으면 배가 고프다.", Item.ItemType.DryFood, 0));
            //레벨 시스템 확인용.
            items.Add(new Item("08", 8, "아무튼약초", 100, "아무튼 몸에 좋은 약초. 정제된 물약은 비싸기 때문에 가난한 모험가들이 많이 찾는다.", Item.ItemType.Herb,0));
            items.Add(new Item("09", 9, "아무튼물약", 800, "각종 통증을 가라앉혀주는 약. 무슨 증상이든 아무튼 이 약을 먹으라는 말에서 이름이 유래되었다.", Item.ItemType.Potion, 0, 8));
            items.Add(new Item("10", 10, "말린 과일", 500, "말린 과일. \n여행할 때의 간식거리. 없으면 상큼함이 부족할지도.", Item.ItemType.DryFood, 0));
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void InitGame()
    {
        // 게임 초기화 시 아이템의 갯수도 초기화함
        for(int i = 0; i < items.Count; i++)
        {
            items[i].itemCount = 0;
        }
    }
}
