using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextMoving : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    // 로딩 씬에서 텍스트를 위로 올려주는 클래스.
    // 터치시 스크롤 속도를 올리고 떼면 원상복구함.

    GameObject text;
    int scrollSpeed;
    public bool flag;

    public void OnPointerDown(PointerEventData eventData)
    {
        scrollSpeed = 10;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        scrollSpeed = 5;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    // Start is called before the first frame update
    void Awake()
    {
        flag = false;
        scrollSpeed = 5;
        text = GameObject.Find("Text");
        text.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, -(Screen.height / 2 + 500), 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (text.GetComponent<RectTransform>().localPosition.y < Screen.height / 2+200)
            text.GetComponent<RectTransform>().localPosition = new Vector2(0.0f, text.GetComponent<RectTransform>().localPosition.y + scrollSpeed);
        else
        {
            FindObjectOfType<LoadingSceneManager>().SetFlag();
        }
    }
}
