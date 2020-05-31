using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 게임을 전체적으로 관리해주는 클래스.
    // 게임의 현재 상태를 curState를 통해 관리하고 있으며 조건을 충족하거나, 씬이 바뀔경우 State 역시 바뀜.
    // 게임내의 정보를 보관하고 있으며, 게임 내용은 xml 파일을 이용하여 저장하고 로드함
    // 싱글톤 클래스로 전체 게임 내부에서 단 하나만 존재할 수 있으며, 다른 씬에서도 존재해야 하기 때문에 DontDestroyOnLoad()함수로 파괴 되지않도록 함


    public static GameManager instance;

    public enum State { start, market, selling, tutorial, Ending };
    public State curState;                      // 현재 상태

    public int[] inventory = new int[20];       // 인벤토리 슬롯에 어떤 아이템이 들어있는지 저장하는 배열
    public int[,] shelves;                      // 몇번 shelf에 몇번 아이템이 몇개 들어가있는지 저장하는 배열 ex> shelves[i,j] = k i번 shelf에 j번 아이템이 k개 들어있다.
    public int[] result;                        // 하루의 판매가 끝날 때 정산창을 보여주기 위해 아이템이 몇개 팔렸는지 저장하는 배열

    public int iNum;                            // 아이템 종류의 수
    public int Gold;
    public int level;
    public int exp;
    public int curExp;
    public int NPC;                             // Market씬에서 터치된 NPC에 따라 대사를 다르게 내보내기 위한 변수

    public bool objectflag;                     // Start씬 및 Selling 씬에서 손님들이 생성되도 될때 true, 아니면 false
    public bool flag;                           // Selling 씬에서 판매 시작될때 true
    public bool isParse;                        // Text로 된 Script를 읽어들일 때 true, 그 외에는 false
    public bool nextScirpt;                     // 읽어들인 Script의 다음 내용을 표시할 때 true
    
    public GameObject tutorial;                 // 튜토리얼 말풍선.
    public AudioSource soundEffect;

    private TextAsset data;                     // 읽어들인 텍스트 전체를 저장하는 변수
    private string[] strs;                      // 읽어들인 텍스트를 개행 단위로 쪼개서 저장하는 배열
    private int index;                          // 위 배열의 인덱스
    private int parseNum;
    

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            InitGame();
        }
        else if(instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                // 종료 UI켜는 함수
                UImanager.instance.QuitUIOn();
            }
        }
        switch(curState)
        {
            case State.start:
                if (objectflag)
                {
                    // 랜덤한 손님 캐릭터가 가게로 들어가는 연출을 위한 부분
                    objectflag = false;
                    StartCoroutine(CoolDown(10.0f,0));
                    int x = Random.Range(0, 4);
                    GameObject obj = ObjectManager.instance.PopObject(x);
                    obj.GetComponent<Character>().x = x;
                    obj.transform.position = new Vector3(-2, 5, -55);
                    obj.SetActive(true);
                }
                break;
            case State.selling:
                // 게임오버 조건 충족시 (스토리를 구현하지 않아 스토리상의 게임오버는 미구현)
                //if (Input.GetKeyDown(KeyCode.Space)) 
                //{//게임오버 테스트
                //    curState = State.Ending;
                //    SceneManager.LoadScene("Ending");
                //}
                if (objectflag && flag)
                {
                    // 판매 시작 버튼을 눌렀을 때, 손님이 생성되도록 하는 부분
                    objectflag = false;
                    StartCoroutine(CoolDown(6.0f,0));
                    int x = Random.Range(0, 4);
                    GameObject obj = ObjectManager.instance.PopObject(x);
                    obj.GetComponent<Character>().x = x;
                    obj.transform.position = new Vector3(-40, 2, -40);
                    obj.SetActive(true);
                }
                break;
            case State.tutorial:
                if (SceneManager.GetActiveScene().name != "Loading")
                {
                    // 로딩 씬이 끝나기 전에 튜토리얼 UI가 뜨지 않게 하는 부분
                    if (isParse)
                    {
                        // 튜토리얼 UI를 켜고 텍스트를 읽도록 하는 부분
                        GameObject obj = Instantiate(tutorial);
                        obj.transform.SetParent(GameObject.Find("Canvas").transform);
                        obj.name = "Tutorial";
                        obj.transform.GetComponent<RectTransform>().localScale = new Vector3(Screen.width / 1920, Screen.width / 1920, 0);
                        obj.transform.GetComponent<RectTransform>().position = new Vector2(Screen.width / 2, Screen.height / 4);
                        isParse = false;
                        ParseScript(parseNum);
                    }
                    if (nextScirpt)
                    {
                        // 스크립트의 다음 내용을 수행하도록 하는 부분.
                        nextScirpt = false;
                        NextScirpt();
                    }
                }
                break;
            case State.market:
                // NPC의 대사를 처리하는 부분
                if(isParse)
                {
                    isParse = false;
                    ParseScript(3);
                }
                if(nextScirpt)
                {
                    nextScirpt = false;
                    int x = 0;
                    string sayer = "";
                    switch(NPC)
                    {
                        case 0:
                            //약초상
                            //트리거 발동시 (스토리 진행시) , 미구현
                            //if (trigger == 1)
                            //    x = 10;
                            x = Random.Range(0, 3);
                            sayer = "약초상";
                            break;
                        case 1:
                            //식료품점원
                            x = Random.Range(3, 6);
                            sayer = "식료품점원";
                            break;
                        case 2:
                            //연금술사
                            x = Random.Range(6, 9);
                            sayer = "연금술사";
                            break;
                    }
                    GameObject.Find("Canvas").transform.Find("Dialogue").Find("Sayer").GetComponent<Text>().text = sayer;
                    // TypingEffect 스크립트를 재활용하여 구현하려 하였으나 TypingEffect스크립트를 오브젝트에 넣으면 활성화 되지않아서 새로 구현한 스크립트 사용
                    GameObject.Find("Canvas").transform.Find("Dialogue").GetComponent<TypingEffectTest>().txt = strs[x].Replace("\\n", "\n");
                    GameObject.Find("Canvas").transform.Find("Dialogue").GetComponent<TypingEffectTest>().StartTyping();
                }
                break;
        }
    }

    public void SaveFile()
    {
        // xml파일을 이용하여 게임을 저장함.
        // 저장하는 내용은 아이템들의 목록에 있는 각 아이템의 갯수, 
        // inventory배열, shelves배열, 그 외 레벨 및 골드에 관련된 변수들.

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));
        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "SaveInfo", string.Empty);
        xmlDoc.AppendChild(root);
        XmlNode itemList = xmlDoc.CreateNode(XmlNodeType.Element, "ItemList", string.Empty);
        root.AppendChild(itemList);
        for(int i = 0; i < ItemList.instance.items.Count; i++)
        {
            XmlElement itemElements = xmlDoc.CreateElement("Item");
            itemElements.SetAttribute("ItemID", ItemList.instance.items[i].itemID.ToString());
            itemElements.SetAttribute("ItemCount", ItemList.instance.items[i].itemCount.ToString());
            itemList.AppendChild(itemElements);
        }
        XmlNode invenInfo = xmlDoc.CreateNode(XmlNodeType.Element, "Inventory", string.Empty);
        root.AppendChild(invenInfo);
        for (int i = 0; i < ItemList.instance.items.Count; i++)
        {
            XmlElement itemElements = xmlDoc.CreateElement("Slot");
            itemElements.SetAttribute("slotID", i.ToString());
            itemElements.SetAttribute("ItemID", inventory[i].ToString());
            invenInfo.AppendChild(itemElements);
        }
        XmlNode shelvesInfo = xmlDoc.CreateNode(XmlNodeType.Element, "Shelves", string.Empty);
        root.AppendChild(shelvesInfo);
        for (int i = 0; i < 4; i++)
        {
            for(int j = 0; j < ItemList.instance.limit; j++)
            {
                XmlElement itemElements = xmlDoc.CreateElement("Slot");
                itemElements.SetAttribute("ShelfNum", i.ToString());
                itemElements.SetAttribute("ItemID", j.ToString());
                itemElements.SetAttribute("ItemNum", shelves[i,j].ToString());
                shelvesInfo.AppendChild(itemElements);
            }
        }
        XmlNode l = xmlDoc.CreateNode(XmlNodeType.Element, "LevelInfo", string.Empty);
        root.AppendChild(l);
        XmlElement levelInfo = xmlDoc.CreateElement("LevelInfo");
        levelInfo.SetAttribute("level", level.ToString());
        levelInfo.SetAttribute("curExp", curExp.ToString());
        levelInfo.SetAttribute("Exp", exp.ToString());
        levelInfo.SetAttribute("Gold", Gold.ToString());
        levelInfo.SetAttribute("limit", ItemList.instance.limit.ToString());
        l.AppendChild(levelInfo);
    #if UNITY_EDITOR
        xmlDoc.Save(Application.dataPath + "/Save.xml");
    #endif
    #if UNITY_ANDROID
        xmlDoc.Save(Application.persistentDataPath + "/Save.xml");
    #endif
    }

    void LoadFile()
    {
        // 저장한 xml파일을 읽어들여 저장된 상태로 돌리는 부분.

        string filepath;
        XmlDocument xmlDoc = new XmlDocument();
    #if UNITY_EDITOR
        filepath = Application.dataPath + "/Save.xml";
    #endif
    #if UNITY_ANDROID
        filepath = Application.persistentDataPath + "/Save.xml";
    #endif
        xmlDoc.Load(filepath);
        XmlNode node = xmlDoc.SelectSingleNode("SaveInfo/ItemList");
        foreach (XmlElement xmlElement in node)
        {
            int tmp = System.Convert.ToInt32(xmlElement.GetAttribute("ItemID"));
            ItemList.instance.items[tmp].itemCount
                = System.Convert.ToInt32(xmlElement.GetAttribute("ItemCount"));
        }
        node = xmlDoc.SelectSingleNode("SaveInfo/Inventory");

        foreach (XmlElement xmlElement in node)
        {

            int tmp = System.Convert.ToInt32(xmlElement.GetAttribute("slotID"));
            inventory[tmp] = System.Convert.ToInt32(xmlElement.GetAttribute("ItemID"));
        }
        node = xmlDoc.SelectSingleNode("SaveInfo/Shelves");

        foreach (XmlElement xmlElement in node)
        {
            int tmp1 = System.Convert.ToInt32(xmlElement.GetAttribute("ShelfNum"));
            int tmp2 = System.Convert.ToInt32(xmlElement.GetAttribute("ItemID"));
            shelves[tmp1, tmp2] = System.Convert.ToInt32(xmlElement.GetAttribute("ItemNum"));
        }
        node = xmlDoc.SelectSingleNode("SaveInfo/LevelInfo");
        foreach (XmlElement xmlElement in node)
        {
            level = System.Convert.ToInt32(xmlElement.GetAttribute("level"));
            curExp = System.Convert.ToInt32(xmlElement.GetAttribute("curExp"));
            exp = System.Convert.ToInt32(xmlElement.GetAttribute("Exp"));
            Gold = System.Convert.ToInt32(xmlElement.GetAttribute("Gold"));
            ItemList.instance.limit = System.Convert.ToInt32(xmlElement.GetAttribute("limit"));
        }
    }

    void InitGame()
    {
        // 게임이 시작되거나 게임 오버된 후 새로 시작할 때 수행하는 함수.
        curState = State.start;
        iNum = ItemList.instance.items.Count;
        ItemList.instance.InitGame();
        shelves = new int[4, iNum];
        result = new int[iNum];
        for (int i = 0; i < 20; i++)
        {
            inventory[i] = -1;
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < ItemList.instance.limit; j++)
            {
                shelves[i, j] = 0;
            }
        }
        for (int i = 0; i < iNum; i++)
        {
            result[i] = 0;
        }
        Gold = 10000;
        objectflag = true;
        flag = false;
        isParse = false;
        nextScirpt = false;
        index = 0;
        parseNum = 0;
        level = 1;
        exp = 1000;
        curExp = 0;
        parseNum = 0;
        data = null;
        soundEffect = gameObject.AddComponent<AudioSource>();
        soundEffect.loop = false;
    }

    public int ExpUP(int num)
    {
        // 물품 판매 시간이 끝나고 결과창을 띄울 때 경험치를 채워주는 함수.
        curExp += num;
        int l = 0;
        while (curExp >= exp)
        {
            curExp -= exp;
            exp = exp * 6 / 5;
            l += 1;
            soundEffect.clip = Resources.Load<AudioClip>("SoundEffect/Level-up-sound-effect");
            soundEffect.Play();
        }
        level += l;
        if (level > 2 && level - l <= 2)
        {
            // 최초로 레벨이 일정 이상 도달 시 아이템 종류 해금.
            // 추후 아이템 추가시 숫자 대신 변수를 사용하여 변수를 조절할 경우 계속된 해금이 가능
            ItemList.instance.limit = 11;
            ItemList.instance.flag = true;
        }
        return l;
    }

    public void StartSelling()
    {
        // 판매를 시작하는 함수.
        if(curState == State.selling)
        {
            flag = true;
            soundEffect.clip = Resources.Load<AudioClip>("SoundEffect/Bell");
            soundEffect.Play();
            for (int i = 0; i < iNum; i++)
            {
                // 결과 창 초기화
                result[i] = 0;
            }
            // 판매 시간을 30초로 지정.
            StartCoroutine(CoolDown(30.0f, 1));
        }
    }

    public void LoadScene()
    {
        // 현 상태에 따라 씬을 불러내는 함수.
        switch(curState)
        {
            case State.start:
                //게임 시작 시 세이브 파일 초기화.
                SaveFile();
                curState = State.tutorial;
                objectflag = false;
                LoadingSceneManager.LoadScene("Selling");
                break;
            case State.selling:
                curState = State.market;
                SceneManager.LoadScene("Market");
                break;
            case State.market:
                curState = State.selling;
                SceneManager.LoadScene("Selling");
                break;
            case State.Ending:
                curState = State.start;
                // 게임 오버 후 처음 화면으로 돌아갈 때 게임을 초기화함.
                InitGame();
                SceneManager.LoadScene("Start");
                break;
        }
    }

    public void GameLoad()
    {
        // 첫 화면에서 불러오기를 택했을 때 수행
        curState = State.selling;
        objectflag = false;
        SceneManager.LoadScene("Selling");
        LoadFile();
    }

#region parseScript
    // txt파일을 읽어들여 그 내용을 수행하도록 하는 부분.
    void ParseScript(int num)
    {
        // 파일을 처음 읽어들여 strs배열에 개행단위로 저장하는 부분
        nextScirpt = true;

        switch (num)
        {
            // 입력된 숫자에 따라 어떤 txt파일을 읽을지 결정
            case 0:
                if(data == null)
                {
                    data = Resources.Load("TextScript/tutorial0", typeof(TextAsset)) as TextAsset;
                    strs = data.text.Split('\n');
                }
                break;
            case 1:
                if (data == null)
                {
                    data = Resources.Load("TextScript/tutorial1", typeof(TextAsset)) as TextAsset;
                    strs = data.text.Split('\n');
                }
                break;
            case 2:
                if (data == null)
                {
                    data = Resources.Load("TextScript/tutorial2", typeof(TextAsset)) as TextAsset;
                    strs = data.text.Split('\n');
                }
                break;
            case 3:
                if (data == null)
                {
                    data = Resources.Load("TextScript/NPCScript", typeof(TextAsset)) as TextAsset;
                    strs = data.text.Split('\n');
                }
                break;
        }
        
    }
    void NextScirpt()
    {
        // 스크립트의 내용을 수행하고 다음 스크립트로 넘어가는 함수
        string str = strs[index];
        // 입력된 스크립트의 첫 4자는 기능, 그 뒤는 내용으로 지정하여 설계함.
        if (str.Substring(0, 4) == "word")
        {
            // 대사인 경우
            FindObjectOfType<TypingEffect>().txt = str.Substring(5).Replace("\\n", "\n");
            FindObjectOfType<TypingEffect>().StartTyping();
        }
        else if (str.Substring(0, 4) == "func")
        {
            // 함수를 수행해야 할경우.
            SendMessage(str.Split(' ')[1], str.Split(' ')[2]);  // 띄어쓰기로 구분된 내용을 split으로 구분 후 사용. 마지막 단어 뒤에 띄어쓰기가 없으면 비정상적으로 작동함.
            nextScirpt = true;
        }
        else if(str.Substring(0,4) == "endt")
        {
            TutorialEnd();
        }
        index++;
        if (index >= strs.Length)
        {
            // 읽어들인 파일을 다 수행했을때 다음 스크립트 읽기 위해 세팅
            Destroy(GameObject.Find("Canvas").transform.Find("Tutorial").gameObject);
            index = 0;
            parseNum++;
            data = null;
        }
        else if (!nextScirpt)
            FindObjectOfType<TypingEffect>().SetFlag();
    }
    
    void Tutorial01(string str)
    {
        GameObject.Find("Canvas").transform.Find("Tutorial").gameObject.SetActive(false);
        SceneManager.LoadScene(str);
        isParse = true;
    }

    void Tutorial02(string str)
    {
        GameObject.Find("Main Camera").GetComponent<CameraMove>().SetTarget(System.Convert.ToInt32(str));
    }

    public void TutorialEnd()
    {
        Destroy(GameObject.Find("Canvas").transform.Find("Tutorial").gameObject);
        curState = State.selling;
        data = null;
    }
#endregion

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator CoolDown(float cooltime, int type)
    {
        float original = cooltime;
        //쿨다운 코루틴
        while (cooltime > 0.0f)
        {
            cooltime -= Time.deltaTime;
            if (type == 1)
                // ClockBehavior에 timer변수 조절로 시계 작동시킴
                GameObject.Find("Clock").GetComponent<ClockBehavior>().timer = cooltime / original;
            yield return new WaitForFixedUpdate();
        }
        if(type == 0)
            objectflag = true;
        else if(type ==1)
        {
            objectflag = false;
            flag = false;
            // 파는 단계 끝내는 함수 호출
            UImanager.instance.ResultUIOn();
        }
    }
}
