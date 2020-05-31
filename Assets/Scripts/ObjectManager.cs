using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    // 손님 오브젝트를 매번 Instantiate하는 부하를 줄이기 위하여 사용하는 오브젝트 풀 클래스
    // 하나만 존재해야하기때문에 싱글톤 클래스로 구현

    public static ObjectManager instance;
    public int objectKind;                      // 들어올 오브젝트의 종류의 수
    public GameObject[] objectList;
    public int[] objectNum;
    public Stack<GameObject>[] objectStack;     // 오브젝트 저장하는 스택
    GameManager gameManager;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gameManager = FindObjectOfType<GameManager>();
            objectStack = new Stack<GameObject>[objectKind];
            if(objectKind != 0)
            {
                for (int i = 0; i < objectKind; i++)
                {
                    objectStack[i] = new Stack<GameObject>();
                    for (int j = 0; j < objectNum[i]; j++)
                    {
                        //정해진 갯수 만큼 오브젝트 미리 생성.
                        GameObject obj = Instantiate(objectList[i]);
                        obj.transform.SetParent(transform);
                        obj.SetActive(false);
                        objectStack[i].Push(obj);
                    }
                }
            }
            
            
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject PopObject(int x)
    {
        // 오브젝트 풀에서 오브젝트를 내보내는 함수
        if (objectStack[x].Count <= 0)
        {
            // 오브젝트 풀에 오브젝트가 모자랄 경우 예외처리
            for (int j = 0; j < objectNum[x]; j++)
            {
                GameObject tmp = Instantiate(objectList[0]);
                tmp.SetActive(false);
                objectStack[x].Push(tmp);
            }
        }
        GameObject obj = objectStack[x].Pop();
        obj.GetComponent<Character>().Init();
        return obj;
    }

    public void PushObject(GameObject obj, int objectKind)
    {
        //오브젝트 풀에 오브젝트를 다시 넣는 함수
        obj.SetActive(false);
        objectStack[objectKind].Push(obj);
    }

    
}
