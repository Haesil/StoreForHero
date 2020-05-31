using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingEffectTest : MonoBehaviour
{
    // Market 씬에서 NPC의 대사를 한글자씩 출력해주는 클래스
    // TypingEffect 스크립트를 재활용하려 하였으나 TypingEffect스크립트를 오브젝트에 넣으면 오브젝트가 활성화 되지않고 해결 방법을 찾지 못해 새로 구현한 스크립트 사용
    // 해결 시 수정 예정
    public string txt;
    Text m_text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Typing()
    {

        for (int i = 0; i < txt.Length; i++)
        {
            m_text.text += txt[i];
            yield return new WaitForSeconds(0.1f);
        }
        StopCoroutine("Typing");
    }

    public void StartTyping()
    {
        m_text = transform.Find("Text").GetComponent<Text>();
        m_text.text = "";
        StartCoroutine("Typing");
    }
    
}
