using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickEvent : MonoBehaviour, IPointerClickHandler
{
    // 엔딩씬에서 터치하면 맵 처음으로 돌아가도록 하기 위해 구현한 클래스

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.LoadScene();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
