using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    // 해상도를 고정시키는 클래스
    // 16:9로 해상도를 항상 맞춰주고 빈 자리는 검은색으로 칠해줌.
    void Awake()
    {
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;

        float widthRate = (float)Screen.width / 16.0f;
        float heightRate = (float)Screen.height / 9.0f;

        float widthPad = 0;
        float heightPad = 0;
        if (widthRate < heightRate)
        {
            heightPad = (widthRate / heightRate - 1) / 2;
        }
        else
        {
            widthPad = (heightRate / widthRate - 1) / 2;
        }

        rect.x += System.Math.Abs(widthPad);
        rect.y += System.Math.Abs(heightPad);
        rect.width += widthPad * 2;
        rect.height += heightPad * 2;

        camera.rect = rect;
    }

    // 해상도는 고정되지만 UI나 배경들이 레터박스 부분에 잘려서 추가되는 경우가 자꾸 생겨서 검색하여 추가한 부분.
    void OnPreCull() => GL.Clear(true, true, Color.black);

    // Update is called once per frame
    void Update()
    {
        
    }
}
