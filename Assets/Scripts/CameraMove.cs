using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // Market Scene에서 NPC를 터치했을 때 카메라가 NPC를 향해 이동하며 회전하는 기능을 구현한 클래스
    // NPC를 클릭했을 때 UIManager로부터 타겟 넘버를 받아 타겟을 설정하고 그 타겟에 따라 목표 지점으로 이동하도록 함.
    // 이동시에는 회전과 이동이 자연스럽게 보이기 위해 Lerp를 이용하여 구현해야 함.

    private Vector3 defaultPos;                                 // 카메라의 원래 위치를 저장하는 변수
    private Quaternion defaultRot;                              // 카메라의 원래 Quaternion값을 저장하는 변수
    private Vector3 grocery = new Vector3(31,9,2);              // 식료품점원을 터치했을 때 카메라가 이동할 위치
    private Vector3 alchemist = new Vector3(9.5f, 9, -11);      // 연금술사를 터치했을 때 카메라가 이동할 위치
    private Vector3 HerbSeller = new Vector3(0,9,-23.5f);       // 약초상을 터치했을 때 카메라가 이동할 위치
    private GameObject target;                                  // 카메라가 비출 타겟.
    private int targetNum;

    public int rotSpeed;
    public int moveSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        defaultPos = transform.position;
        defaultRot = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target)
        {
            // 타겟이 정해졌을 경우
            switch(targetNum)
            {
                case 1:
                    // 타겟이 약초상인 경우
                    transform.position = Vector3.Lerp(transform.position, HerbSeller, moveSpeed * Time.deltaTime);
                    break;
                case 2:
                    // 타겟이 식료품점원인 경우
                    transform.position = Vector3.Lerp(transform.position, grocery, moveSpeed * Time.deltaTime);
                    break;
                case 3:
                    // 타겟이 연금술사인 경우
                    transform.position = Vector3.Lerp(transform.position, alchemist, moveSpeed * Time.deltaTime);
                    break;
            }
            // 카메라가 타겟을 바라보게 하는 부분
            Vector3 dir = target.transform.position - transform.position;
            // y좌표를 0으로 하지않으면 anchorpositon을 바라보기 때문에 그렇지 않도록 0으로 바꿔줌.
            dir.y = 0;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);
        }
        else
        {
            // 타겟이 정해지지 않은 경우 원래 위치로 돌아감.
            transform.position = Vector3.Lerp(transform.position, defaultPos, moveSpeed *2 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, defaultRot, rotSpeed*2 * Time.deltaTime);
        }
    }

    public void SetTarget(int num)
    {
        switch (num)
        {
            case 0:
                target = null;
                targetNum = 0;
                break;
            case 1:
                target = GameObject.Find("HerbSeller");
                targetNum = 1;
                break;
            case 2:
                target = GameObject.Find("grocery");
                targetNum = 2;
                break;
            case 3:
                target = GameObject.Find("alchemist");
                targetNum = 3;
                break;
        }

    }
}
