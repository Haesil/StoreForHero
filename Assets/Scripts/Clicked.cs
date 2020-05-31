using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Clicked : MonoBehaviour, IPointerClickHandler
{
    // 레벨업 창을 터치하면 사라지도록 구현한 클래스
    // 레벨업 창을 초기화한 후 비활성화.

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.Find("Original").GetComponent<Text>().text = "";
        transform.Find("Changed").GetComponent<Text>().text = "";
        transform.Find("Text").GetComponent<Text>().text = "";
        gameObject.SetActive(false);
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
