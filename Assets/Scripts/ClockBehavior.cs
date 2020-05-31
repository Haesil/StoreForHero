using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockBehavior : MonoBehaviour
{
    // Selling Scene에서 판매 시작 버튼을 누른 후에 판매 시간이 남은 것을 보여주기 위한 클래스
    // 총 판매시간을 현재 남은 판매시간으로 나눈 timer 변수를 360에 곱하여 회전을 변경시킴.

    public float timer;
    public GameObject needle;
    
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        needle.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 360 * timer);
    }
}
