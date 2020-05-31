using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    // 다음 씬 로딩할 때 임시로 띄우는 씬 구현
    // 스토리를 띄우는 용도로도 사용하였음.

    static string nextScene;
    public bool flag;           // 스토리가 스킵되거나 스토리를 다 보여준 경우 true
    
    void Awake()
    {
        StartCoroutine(LoadScene());
    }
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;

        SceneManager.LoadScene("Loading");
    }
    public void SetFlag()
    {
        GameManager.instance.isParse = true;
        flag = true;
    }
    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        
        while(!op.isDone)
        {
            yield return null;
            
            if(op.progress >= 0.9f)
            {
                GameObject.Find("Canvas").transform.Find("Button").gameObject.SetActive(true);
                if (flag)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
