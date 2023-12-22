using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (Instance == null)
            {
                //하이라이어키에서 오브젝트 찾기
                Instance = (T)FindObjectOfType(typeof(T));
                //그래도 없다면 싱글톤 오브젝트 만들기 
                if (Instance == null)
                {
                    GameObject singletoneObj = new GameObject(typeof(T).Name, typeof(T));
                    instance = singletoneObj.GetComponent<T>();
                }
            }
            return Instance;
        }
        set { instance = value; }
    }


    public virtual void Initialization()
    {
        //싱글톤 스크립트가 달린 오브젝트에 부모가 있을 경우 부모를 DontDestroyOnLoad
        if (transform.parent != null && transform.root != null) DontDestroyOnLoad(this.transform.root.gameObject);
        else DontDestroyOnLoad(this.gameObject);
    }
    public virtual void Clear()
    {
        Instance = null;
    }
}
