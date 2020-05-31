using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speech : MonoBehaviour
{
    // 손님 오브젝트가 원하는 아이템이 배치되지 않았을 때 출력되는 말풍선이 캐릭터를 따라가도록 하는 클래스
    // 손님 오브젝트의 WorldPosition을 ScreenPosition으로 변환하여 그 위치를 따라다니도록 함.

    RectTransform rect;
    public GameObject obj;
    Canvas canvas;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>();

        // 일정 시간이 지나면 파괴함.
        Invoke("DestroySelf", 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(obj.transform.position);
        screenPos.x += 250;
        screenPos.y += 200;
        rect.position = screenPos;
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
