using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    // Selling Scene에서 판매 시작 버튼을 누른 뒤에 생성되어 유저가 세팅한 물품을 향해 이동하는 캐릭터를 구현한 클래스
    // 캐릭터를 움직이는 애니메이션은 StandardAsset을 이용하였지만 Animator에 움직임을 전달하는 스크립트는 직접 구현하였음.
    // NavMeshAgent를 사용하여 목표한 물품이 있는 Shelf의 좌표를 향해 이동하도록 함.
    // 목표하는 아이템은 난수 생성으로 결정.
    // 코루틴을 사용하여 목표지점 도달 후 일정 시간을 기다리도록 한 후 나가는 모션을 취한 뒤 Object Pool에 반납.
    // 만약 목표하는 아이템을 유저가 세팅해놓지 않았을 경우 원하는 아이템이 없다는 말풍선을 띄운 후 나가는 모션을 취한 뒤 Object Pool에 반납.

    private NavMeshAgent m_agent;
    private Animator m_animator;
    private Vector3 moveDir;
    private float m_turnAmount;
    private int shelf;
    private bool characterflag;                 // 첫번째 목표물에 도착했을 시 false
    private bool flag;                          // 두번째 목표물에 도착했을 때 true

    public int x;
    public int item;
    public GameObject m_target;
    public GameObject speechbubble;
    public GameObject speech;
    public float m_stopDistance;

    // Start is called before the first frame update
    void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();

        shelf = -1;

        characterflag = true;
        flag = false;

        m_stopDistance = 8.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.curState == GameManager.State.selling && !GameManager.instance.flag)
            ObjectManager.instance.PushObject(gameObject, x);
        else
        {
            if (m_target != null)
            {
                // 타겟이 정해져 있을때
                float distance = Vector3.Distance(m_target.transform.position, transform.position);

                if (distance <= m_stopDistance)
                {
                    // 타겟이 가까이 있을때 정지하도록 하는 함수.
                    m_agent.SetDestination(transform.position);

                    if (characterflag)
                    {
                        if (shelf != -1)
                        {
                            Shelf.instance.BuyItem(shelf, item, 1);
                            GameManager.instance.result[item]++;
                            GameManager.instance.Gold += ItemList.instance.items[item].itemPrice;
                            characterflag = false;
                        }
                        else
                        {
                            if (GameManager.instance.curState == GameManager.State.selling)
                            {
                                // 오프닝 씬에서 캐릭터가 걸어서 상점으로 가는 연출을 꾸몄기 때문에 Selling 씬에서만 말풍선을 표시하도록 하였음.
                                speechbubble.GetComponent<Speech>().obj = gameObject;
                                speechbubble.transform.Find("Text").GetComponent<Text>().text = ItemList.instance.items[item].itemName + "의 재고가 없네";
                                speech = Instantiate(speechbubble);
                                speech.transform.SetParent(GameObject.Find("Canvas").transform);
                            }
                            characterflag = false;
                        }
                        StartCoroutine(CoolDown(3.0f, 0));
                    }
                    if (flag)
                    {
                        StartCoroutine(CoolDown(1.0f, 1));
                    }
                }
                else
                {
                    m_agent.SetDestination(m_target.transform.position);
                }
            }
            moveDir = m_agent.desiredVelocity;
            Move();
        }
        
    }
    
    public void Move()
    {
        // 이동 Vector3를 받아 애니메이터에 전달하는 함수
        moveDir = transform.InverseTransformDirection(moveDir);
        m_turnAmount = Mathf.Atan2(moveDir.x, moveDir.z);                       // 회전하는 각도를 구함
        m_animator.SetFloat("Forward", moveDir.z, 0.1f, Time.deltaTime);
        m_animator.SetFloat("Turn", m_turnAmount, 0.1f, Time.deltaTime);
    }

    public void Init()
    {
        
        if(GameManager.instance.curState == GameManager.State.selling)
        {
            item = Random.Range(3, ItemList.instance.limit);
        }
        SetTarget(item);

        characterflag = true;
    }

    public void SetTarget(int item)
    {

        if (GameManager.instance.curState == GameManager.State.start)
        {
            m_target = GameObject.FindWithTag("Player");
        }
        else
        {
            // 창고 스크립트에서 원하는 아이템 위치 찾고 그 위치를 타겟
            for(int i = 0; i < 4; i++)
            {
                if(GameManager.instance.shelves[i,item] > 0)
                {
                    shelf = i;
                    break;
                }
            }
            // 원하는 아이템이 있는 위치를 타겟으로 삼도록 함
            switch (shelf)
            {
                case 0:
                    m_target = GameObject.Find("shelf1");
                    break;
                case 1:
                    m_target = GameObject.Find("shelf2");
                    break;
                case 2:
                    m_target = GameObject.Find("shelf3");
                    break;
                case 3:
                    m_target = GameObject.Find("shelf4");
                    break;
                default:
                    m_target = GameObject.Find("Default1");
                    break;
            }
        }
    }

    

    IEnumerator CoolDown(float cooltime, int type)
    {
        //쿨다운 코루틴
        while (cooltime > 0.0f)
        {
            cooltime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        if(type == 0)
        {
            // 물건을 구입했거나 물건이 없다는 말풍선을 띄운 후 퇴장할 때
            m_target = GameObject.Find("Default2");
            flag = true;
        }
        else if(type ==1)
        {
            // 퇴장 후 오브젝트 풀에 반납
            ObjectManager.instance.PushObject(gameObject, x);
        }
    }
}
