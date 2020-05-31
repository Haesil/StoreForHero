using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TypingEffect : MonoBehaviour, IPointerDownHandler
{
    // 튜토리얼 UI의 대사를 한글자씩 출력하는 클래스
    // 터치시 Flag를 바꿔 수행하는 내용을 변경함.

    public string txt;
    Text m_text;
    public int flag = 0;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetFlag();
    }

    // Update is called once per frame
    void Update()
    {
        if(flag == 1)
        {
            // 한번더 터치할 경우 코루틴을 멈추고 즉시 출력
            StopCoroutine("Printing");
            m_text.text = txt;
        }
        else if(flag == 2)
        {
            if(GameManager.instance.curState == GameManager.State.tutorial)
                GameManager.instance.nextScirpt = true;
        }
        if (GameManager.instance.curState != GameManager.State.tutorial)
            gameObject.SetActive(false);
    }

    IEnumerator Printing()
    {
        // 한글자씩 출력하기 위한 코루틴
        for (int i = 0; i < txt.Length; i++)
        {
            m_text.text += txt[i];
            yield return new WaitForSeconds(0.1f);
        }
        StopCoroutine("Printing");
        flag = 1;
    }

    public void StartTyping()
    {
        m_text = transform.Find("Text").GetComponent<Text>();
        m_text.text = "";
        StartCoroutine("Printing");
    }

    public void SetFlag()
    {
        flag = (flag + 1) % 3;
    }

    public void TutorialSkip()
    {
        // 튜토리얼 스킵 버튼을 눌렀을 때 수행하는 함수
        GameManager.instance.TutorialEnd();
    }
}
